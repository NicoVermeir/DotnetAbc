using AppSettings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("MyAppConfiguration"));


var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/info", (IConfiguration configuration) =>
{
    var config = new
    {
        Version = configuration["MyAppConfiguration:Version"],
        Environment = configuration["MyAppConfiguration:Environment"],
        Conference = configuration["MyAppConfiguration:CurrentConference"]
    };

    return config;
});

#region scenario

//Binding the configuration to a strongly typed object
app.MapGet("/info2", (IConfiguration configuration) =>
{
    AppConfiguration options = new();
    configuration.GetSection("MyAppConfiguration").Bind(options);

    //of

    AppConfiguration options2 = configuration.GetSection("MyAppConfiguration").Get<AppConfiguration>();

    return options;
});


//via IOptions
app.MapGet("/info3", (IOptions<AppConfiguration> options) =>
{
    return options;
});

#endregion


app.Run();
