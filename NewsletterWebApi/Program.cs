global using NewsletterWebApi.Configuration;
using NewsletterSDK;

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

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
app.UseHttpsRedirection();
app.UseVersioning();
app.UseRateLimit();
app.UseSwaggerConfiguration();
app.UseHealthCheckConfiguration();
app.UseEndpoints();


// var configuration = app.Services.GetRequiredService<IConfiguration>();

// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Console.WriteLine($"Connection String: {connectionString}");

// var enableCache = configuration.GetValue<bool>("Configuration:EnableCache");
// var enableRateLimit = configuration.GetValue<bool>("Configuration:EnableRateLimit");

// Console.WriteLine($"Enable Cache: {enableCache}");
// Console.WriteLine($"Enable Rate Limit: {enableRateLimit}");

app.Run();