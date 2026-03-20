using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/minuteMeals", WebApiVersion1.GetAllMinuteMeals);
app.MapPost("/minuteMeals", WebApiVersion1.CreateMinuteMeal);

app.UseHttpsRedirection();

app.Run();

public static class WebApiVersion1
{
    public static async Task<Ok<MinuteMealDto[]>> GetAllMinuteMeals(MinuteContext context)
    {
        var meals = await context.MinuteMeals.Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price)).ToArrayAsync();

        return TypedResults.Ok(meals);
    }
    
    public static async Task<Created<MinuteMealDto>> CreateMinuteMeal(MinuteContext context)
    {
        var meal = new MinuteMeal { Desc = };

        context.Add(meal);
        await context.SaveChangesAsync();

        MinuteMealDto mealDto = new MinuteMealDto(meal.Id, meal.Desc, meal.Price);
        return TypedResults.Created($"/minuteMeals/{meal.Id}", mealDto);
    }
}
