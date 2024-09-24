using RandomNameGeneratorLibrary;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using FanOutFanIn.Service.Interfaces;
using FanOutFanIn.Service.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        config
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
            .CreateLogger();

        services.AddLogging(lb => lb.AddSerilog(Log.Logger, true));
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<ICalculateProbabilityService, CalculateProbabilityService>();
        services.AddTransient<ISentimentReportGenerator, SentimentReportGenerator>();
        services.AddTransient<IAzureStorageProvider, AzureStorageProvider>();
        services.AddTransient<IPersonNameGenerator, PersonNameGenerator>();
        services.AddTransient<IReportDownloader, ReportDownloader>();

        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
    })
    .Build();

host.Run();
