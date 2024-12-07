using Microsoft.OpenApi.Models;
using CostoReembolsoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

string oracleConnectionString = builder.Configuration.GetConnectionString("OracleDbConnection")!;

builder.Services.AddSingleton(new DatabaseService(oracleConnectionString));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CostoReembolso API",
        Version = "v1",
        Description = "API para gestionar costos de reembolsos y servicios",
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CostoReembolso API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

app.UseRouting();

app.MapControllers();

app.Run();