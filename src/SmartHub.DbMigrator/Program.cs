using Microsoft.EntityFrameworkCore;
using SmartHub.Logic.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLogic(builder.Configuration);

Console.WriteLine("Migrating Db...");

var app = builder.Build();

//await using var scope = app.Services.CreateAsyncScope();

//var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
//await dbContext.Database.MigrateAsync();

Console.WriteLine("DONE!");


app.MapGet("/", c => c.RequestServices.GetService<AppDbContext>().Database.MigrateAsync());

await app.RunAsync();
