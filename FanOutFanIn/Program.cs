using FanOutFanIn.Service.Interfaces;
using FanOutFanIn.Service.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomNameGeneratorLibrary;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<ICalculateProbabilityService, CalculateProbabilityService>();
        services.AddTransient<ISentimentReportGenerator, SentimentReportGenerator>();
        services.AddTransient<IAzureStorageProvider, AzureStorageProvider>();
        services.AddTransient<IPersonNameGenerator, PersonNameGenerator>();
        services.AddLogging();
    })
    .Build();

host.Run();
