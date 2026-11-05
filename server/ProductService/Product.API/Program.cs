using Product.API.Configurations;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Deploy
builder.WebHost.UseUrls("http://0.0.0.0:11117");

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});
var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowCredentials");

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/metrics"), appBuilder =>
{
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

app.UseMetricServer();
app.UseHttpMetrics();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
