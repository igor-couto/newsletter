using System.Net.Sockets;
using System.Diagnostics;
using TechTalk.SpecFlow;
using Testcontainers.PostgreSql;
using Npgsql;
using NewsletterMigrations.Services;

namespace NewsletterIntegrationTests.Hooks;

[Binding]
public class TestSetupHooks
{
    private static PostgreSqlContainer _postgreSqlContainer;

    private const string _databaseName = "NEWSLETTER_TEST_DB";
    private const int _databasePort = 5434;
    private const string _databaseUserName = "admin";
    private const string _databasePassword = "admin";

    private static readonly string _connectionString = $"Server=localhost;Database={_databaseName};Port={_databasePort};User Id={_databaseUserName};Password={_databasePassword};";

    private static ScenarioContext _scenarioContext;

    private static Process _webApiProcess;

    public TestSetupHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        try
        {
            await RunDatabaseTestContainer();
            ApplyMigrations();
            await CleanDatabase();
            await RunWebApi();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{exception.Message}");
            await AfterTestRun();
            throw;
        }
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        StopWebApi();
        await StopDatabaseTestContainer();
    }

    [AfterScenario]
    public static async Task AfterScenario()
    {
        await CleanDatabase();
    }

    private static void ApplyMigrations() 
    {
        MigrationService.MigrateUp(_connectionString);
    }

    private static async Task RunWebApi() 
    {
        if (IsPortInUse(5275))
        {
            Console.WriteLine("The port 5275 is already being used. API might be running. Skipping...");
            return;
        }

        var startInfo = new ProcessStartInfo("dotnet", $"run --urls \"http://localhost:5275\" -- --Configuration:EnableCache false --Configuration:EnableRateLimit false")
        {
            WorkingDirectory = "../../../../NewsletterWebApi",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.EnvironmentVariables["ConnectionStrings__DefaultConnection"] = _connectionString;

        _webApiProcess = new Process { StartInfo = startInfo };
        _webApiProcess.Start();

        using var httpClient = new HttpClient();
        var apiUrl = "http://localhost:5275/health";

        for (var i = 0; i < 10; i++) // retry up to 10 times
        {
            await Task.Delay(1000);
            try
            {
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                    break; // The API is ready
            }
            catch { }
        }   
    }

    private static void StopWebApi() 
    {
        if (_webApiProcess != null && !_webApiProcess.HasExited)
        {
            _webApiProcess.CloseMainWindow();

            if (!_webApiProcess.WaitForExit(5000))
                _webApiProcess.Kill();
         
            _webApiProcess.Dispose();
        }
    }

    public static async Task RunDatabaseTestContainer()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase(_databaseName)
            .WithPortBinding(_databasePort, 5432)
            .WithUsername(_databaseUserName)
            .WithPassword(_databasePassword)
            .WithCleanUp(true)
            .WithAutoRemove(true)
            .Build();

        await _postgreSqlContainer.StartAsync();
    }

    public static async Task StopDatabaseTestContainer()
    {
         await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
        _postgreSqlContainer = null;
    }

    private static bool IsPortInUse(int port)
    {
        try
        {
            using var client = new TcpClient();
            client.Connect("localhost", port);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static async Task CleanDatabase() 
    {
        // TODO: idea: can we just drop the whole schema and created it again?
        //var sql = "DROP SCHEMA public CASCADE; CREATE SCHEMA public; GRANT ALL ON SCHEMA public TO public;";

        var sql = @"
            DELETE FROM publication_deliveries;
            DELETE FROM unsubscriptions;
            DELETE FROM subscribers;
            DELETE FROM publications;
        ";

        using var connection = new NpgsqlConnection(_connectionString);
        var command = new NpgsqlCommand(sql, connection);
        connection.Open();
        await command.ExecuteNonQueryAsync();
    }
}