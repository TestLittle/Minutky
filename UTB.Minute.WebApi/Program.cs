using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/minuteMeals", WebApiVersion1.CreateMinuteMeal);
app.MapGet("/minuteMeals", WebApiVersion1.GetMinuteMeals);
app.MapGet("/minuteMeals/{id:int}", WebApiVersion1.GetMinuteMealById);
app.MapPut("/minuteMeals/{id:int}", WebApiVersion1.UpdateMinuteMeal);
app.MapDelete("/minuteMeals/{id:int}", WebApiVersion1.DeleteMinuteMeal);

app.UseHttpsRedirection();

app.Run();

public static class WebApiVersion1
{
    public static async Task<Created<MinuteMealDto>> CreateMinuteMeal(MinuteMealDto minuteMealDto, MinuteContext context)
    {
        // Add new meal to db

        MinuteMeal m = new() { Desc = minuteMealDto.Desc , Price = minuteMealDto.Price};
        context.MinuteMeals.Add(m);
        await context.SaveChangesAsync();

        MinuteMealDto mDto = new MinuteMealDto(m.Id, m.Desc, m.Price);

        return TypedResults.Created($"/minuteMeals/{mDto.MinuteMealId}", mDto);
    }

    public static async Task<Ok<MinuteMealDto[]>> GetMinuteMeals(MinuteContext context)
    {
        // Return all meals from db
        //2.1 Načíst autory z databáze
        //2.2 Převést je na typ AuthorDto

        MinuteMeal[] mealArr = await context.MinuteMeals.ToArrayAsync();

        //AuthorDto[] authors = new AuthorDto[poleAutoru.Length];

        MinuteMealDto[] minuteMeals = await context.MinuteMeals.Where(m => m.Desc.ToLower().Contains("Smazak"))
                                                   .OrderBy(m => m.Desc)
                                                   .Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price))
                                                   .ToArrayAsync();


        for (int i = 0; i < mealArr.Length; i++)
        {
            minuteMeals[i] = new MinuteMealDto(mealArr[i].Id, mealArr[i].Desc, mealArr[i].Price);
        }


        return TypedResults.Ok(minuteMeals);
    }

    public static async Task<Results<NotFound, Ok<MinuteMealDto>>> GetMinuteMealById(int id,  MinuteContext context)
    {
        // Return meal by id

        if (await context.MinuteMeals.FindAsync(id) is MinuteMeal minuteMeal)
        {
            MinuteMealDto minuteMealDto = new(minuteMeal.Id, minuteMeal.Desc, minuteMeal.Price);

            return TypedResults.Ok(minuteMealDto);
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> UpdateMinuteMeal(int id, MinuteMealDto minuteMealDto, MinuteContext context)
    {
        if (await context.MinuteMeals.FindAsync(id) is MinuteMeal minuteMeal)
        {
            // Change meal in db

            minuteMeal.Desc = minuteMealDto.Desc;

            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> DeleteMinuteMeal(int id, MinuteContext context)
    {
        if (await context.MinuteMeals.FindAsync(id) is MinuteMeal minuteMeal)
        {
            // Delete meal from db

            context.MinuteMeals.Remove(minuteMeal);

            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }
}
