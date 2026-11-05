using Prometheus;
using Serilog;
using UserManagement.API.Configurations;
using UserManagement.API.Helpers;
using UserManagement.API.Messaging.Consumers;
using UserManagement.API.Messaging.KafkaMessage;

var builder = WebApplication.CreateBuilder(args);

// Deploy
builder.WebHost.UseUrls("http://0.0.0.0:11113");

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSignalR();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var kafkaConfig = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>();
builder.Services.AddSingleton(kafkaConfig);
builder.Services.AddHostedService<ConsumerBackgroundService>();
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
    });
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowCredentials");
app.UseRouting();
app.UseSerilogRequestLogging();
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/metrics"), appBuilder =>
{
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

app.UseMetricServer();
app.UseHttpMetrics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/product-signalr/notificationHub");
});

app.MapControllers();

app.Run();
