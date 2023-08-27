using ApiDbCache.Db;
using ApiDbCache.Db.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var dbname = Environment.GetEnvironmentVariable("DB_NAME");
var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbHost = Environment.GetEnvironmentVariable("POSTGRESQL_CONTAINER_NAME");

builder.Services.AddDbContext<ApplicationDatabase>(optionsAction=> optionsAction.UseNpgsql($"Server={dbHost};Database={dbname};User Id={dbUsername};Password={dbPassword}"));
await using (var scope = builder.Services.BuildServiceProvider().CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetService<ApplicationDatabase>();
    await db?.Database.MigrateAsync()!;
}

// builder.Services.AddDistributedMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapGet("/GetStudents", async (ApplicationDatabase db) =>
    {
        var books = await db.Students.ToListAsync();
        return  books;
    })
    .WithName("GetStudents")
    .WithOpenApi();

app.MapGet("/GetEnvironmentVariables", async (IConfiguration db) =>
    {
        var variabes = Environment.GetEnvironmentVariables();
        
        variabes.Add("testWithSemiColon", db["Test:Test1"]);
        return variabes;
    })
    .WithName("GetEnvironmentVariables")
    .WithOpenApi();


app.MapPost("/AddStudents", async (ApplicationDatabase db, [FromBody]Student student) =>
    {
        await db.Students.AddAsync(student);
        var saved = await db.SaveChangesAsync();
        return  saved;
    })
    .WithName("AddStudents")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
