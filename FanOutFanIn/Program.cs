using FanOutFanIn.Service.Interfaces;
using FanOutFanIn.Service.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomNameGeneratorLibrary;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(config =>{
        config.
            AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<ICalculateProbabilityService, CalculateProbabilityService>();
        services.AddTransient<ISentimentReportGenerator, SentimentReportGenerator>();
        services.AddTransient<IAzureStorageProvider, AzureStorageProvider>();
        services.AddTransient<IPersonNameGenerator, PersonNameGenerator>();
        services.AddLogging();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
    })
    .Build();

host.Run();
