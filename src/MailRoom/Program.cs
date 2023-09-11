using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
        theme: AnsiConsoleTheme.Literate
    )
    .WriteTo.File ("logs/hosts-.log",
        fileSizeLimitBytes: 1_000_000,
        rollOnFileSizeLimit: true,
        rollingInterval: RollingInterval.Day,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    )
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .ConfigureHttpJsonOptions((options) =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddControllers((options) => {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        });

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen((options) => options.EnableAnnotations());

    builder.Services.AddSerilog((cfg) =>
    {
        cfg
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                theme: AnsiConsoleTheme.Literate
            )
            .WriteTo.File("logs/application-.log",
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            )
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI((options) => {
            options.SwaggerEndpoint("v1/swagger.json", "Mailroom API");
        });
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex) {
    Log.Fatal(ex, "Host Terminated Unexpectedly");
}
finally {
    Log.CloseAndFlush();
}