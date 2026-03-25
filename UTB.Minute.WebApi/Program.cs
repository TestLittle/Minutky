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
//app.MapDelete("/minuteMeals/{id}", WebApiVersion1.DeleteMinuteMeal);
app.MapPut("/minuteMeals/{id}", WebApiVersion1.PutMinuteMeal);
app.MapPatch("/minuteMeals/{id}", WebApiVersion1.PatchDescMinuteMeal);
app.MapPatch("/minuteMeals/{id}", WebApiVersion1.PatchPriceMinuteMeal);

app.UseHttpsRedirection();

app.Run();

public static class WebApiVersion1
{
    public static async Task<Ok<MinuteMealDto[]>> GetAllMinuteMeals(MinuteContext context)
    {
        var meals = await context.MinuteMeals.Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price, m.IsDeactivated)).ToArrayAsync();

        return TypedResults.Ok(meals);
    }
    
    public static async Task<Created<MinuteMealDto>> CreateMinuteMeal(MinuteMealRequestDto request, MinuteContext context)
    {
        var meal = new MinuteMeal { Desc = request.Desc, Price = request.Price, IsDeactivated = request.IsDeactivated};

        context.MinuteMeals.Add(meal);
        await context.SaveChangesAsync();

        MinuteMealDto mealDto = new MinuteMealDto(meal.Id, meal.Desc, meal.Price, meal.IsDeactivated);
        return TypedResults.Created($"/minuteMeals/{meal.Id}", mealDto);
    }

    public static async Task<Results<NoContent, NotFound>> DeactivateMinuteMeal(int id, MinuteContext context)
    {
        if(await context.MinuteMeals.FindAsync(id) is MinuteMeal meal)
        {
            meal.IsDeactivated = true;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> PutMinuteMeal(int id, MinuteMealRequestDto request, MinuteContext context)
    {

    }


    public static async Task<Results<NoContent, NotFound>> PatchDescMinuteMeal(int id, MinuteMealPatchDescDto patchDesc, MinuteContext context)
    {

    }

    public static async Task<Results<NoContent, NotFound>> PatchPriceMinuteMeal(int id, MinuteMealPatchPriceDto patchDesc, MinuteContext context)
    {

    }


}
