using System.Net;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    var hostName = Dns.GetHostName();
    siloBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "Cluster";
            options.ServiceId = "Service";
        })
        .Configure<SiloOptions>(options =>
        {
            options.SiloName = "Silo";
        })
        .ConfigureEndpoints(
            hostname: hostName,
            siloPort: 11_111,
            gatewayPort: 30_000,
            listenOnAnyHostAddress: true)
        .UseAzureStorageClustering(options => options.ConfigureTableServiceClient(builder.Configuration.GetValue<string>("StorageConnectionString")))
        ;
});

builder.Services.AddWebAppApplicationInsights("Silo");

// uncomment this if you dont mind hosting grains in the dashboard
builder.Services.DontHostGrainsHere();

var app = builder.Build();

app.MapGet("/", () => Results.Ok("Silo"));

app.Run();
