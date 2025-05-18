using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StriveUp.Sync.Application.Interfaces;
using StriveUp.Sync.Application.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IHealthDataProvider, GoogleFitProvider>();
        services.AddHttpClient<GoogleFitProvider>(client =>
        {
            client.BaseAddress = new Uri("https://www.googleapis.com/fitness/v1/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddHttpClient<IUserSyncService, UserSyncService>(client =>
        {
            client.BaseAddress = new Uri("https://striveupapi-emaee9awang6g4ht.polandcentral-01.azurewebsites.net" + "/api/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
