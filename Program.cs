using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.DependencyInjection;
using CostoReembolsoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the Oracle connection string from configuration
string oracleConnectionString = builder.Configuration.GetConnectionString("OracleDbConnection");

// Register DatabaseService with the Oracle connection string
builder.Services.AddSingleton(new DatabaseService(oracleConnectionString));

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger
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

// Configure CORS (if necessary)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// HTTP request pipeline configuration
if (app.Environment.IsDevelopment())
{
    // Enable Swagger only in development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CostoReembolso API V1");
        c.RoutePrefix = string.Empty; // Opens Swagger at the root (optional)
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Use CORS if necessary
app.UseCors("AllowAll");

// Enable routing
app.UseRouting();

// Map controllers to HTTP routes
app.MapControllers();

app.Run();