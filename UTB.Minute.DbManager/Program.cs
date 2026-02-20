using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/reset-db", async (MinuteContext context) =>
{
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    MinuteMeal cheese = new() { Desc = "Smazeny syr s hranolkami", Price = 124.99 };
    MinuteMeal hamburger = new() { Desc = "Hamburger", Price = 139.99 };
    MinuteMeal hamPizza = new() { Desc = "Sunkova Pizza", Price = 109.99 };

    context.MinuteMeals.AddRange(cheese, hamburger, hamPizza);
    
    await context.SaveChangesAsync();
});


app.UseHttpsRedirection();

app.Run();
