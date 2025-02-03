using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OtelAspNet.Components;

var builder = WebApplication.CreateBuilder(args);

// OTEL

//is NULL in this example but can be used for exporters
var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];

var otel = builder.Services.AddOpenTelemetry();


// Configure OpenTelemetry Resources with the application name
otel.ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName));

// Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
otel.WithMetrics(metrics => metrics
    
    // Metrics provider from OpenTelemetry
    .AddAspNetCoreInstrumentation()

    // Metrics provides by ASP.NET Core in .NET 8
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddPrometheusExporter());

// Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    if (tracingOtlpEndpoint != null) // false in this example
    {
        tracing.AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
        });
    }
    else
    {
        tracing.AddConsoleExporter();
    }
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Configure the Prometheus scraping endpoint
app.MapPrometheusScrapingEndpoint();

app.Run();
