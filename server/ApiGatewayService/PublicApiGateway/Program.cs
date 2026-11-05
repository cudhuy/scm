using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Deploy
builder.WebHost.UseUrls("http://0.0.0.0:11116");


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCredentials",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "http://3.106.56.36:8002")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});


// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOcelot();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway V1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowCredentials");
app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();
app.Run();
