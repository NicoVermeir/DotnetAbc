var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.OTelDemo_ApiService>("apiservice");

builder.AddProject<Projects.OTelDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.OtelAspNet>("otelaspnet");

builder.Build().Run();
