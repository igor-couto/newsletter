global using NewsletterWebApi.Configuration;
using NewsletterSDK;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .AddKestrelConfiguration();


var connectionStringssss = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionStringssss}");

builder.Services
    .AddSerializationConfiguration()
    .AddOptions(builder.Configuration)
    .AddNewletterSDKServices()
    .AddCompression()
    .AddVersioning()
    .AddSwagger()
    .AddCache()
    .AddCorsConfiguration()
    .AddRateLimit()
    .AddHealthCheck();

var app = builder.Build();

app.UseCorsConfiguration();
app.UseCompression();
app.UseCache();
app.UseDeveloperExceptionPage();
app.UseVersioning();
app.UseRateLimit();
app.UseSwaggerConfiguration();
app.UseHealthCheckConfiguration();
app.UseEndpoints();

app.Run();