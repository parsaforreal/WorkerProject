using IntervalWorkerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddSingleton<HttpClient>();
    services.AddHostedService<Worker>();
});

var host = builder.Build();

await host.RunAsync();