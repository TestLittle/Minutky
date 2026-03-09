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
        // 🚀 1. Přidání nového autora do databáze.

        MinuteMeal a = new() { Desc = minuteMealDto.Desc , Price = minuteMealDto.Price};
        context.MinuteMeals.Add(a);
        await context.SaveChangesAsync();

        MinuteMealDto aDto = new MinuteMealDto(a.Id, a.Desc, a.Price);

        return TypedResults.Created($"/minuteMeals/{aDto.Id}", aDto);
    }

    public static async Task<Ok<MinuteMealDto[]>> GetMinuteMeals(MinuteContext context)
    {
        // 🚀 2.Vrácení všech autorů z databáze.
        //2.1 Načíst autory z databáze
        //2.2 Převést je na typ AuthorDto

        MinuteMeal[] poleAutoru = await context.MinuteMeals.ToArrayAsync();

        //AuthorDto[] authors = new AuthorDto[poleAutoru.Length];

        MinuteMealDto[] minuteMeals = await context.MinuteMeals.Where(a => a.Desc.ToLower().Contains("am"))
                                                   .OrderBy(a => a.Desc)
                                                   .Select(a => new MinuteMealDto(a.Id, a.Desc, a.Price))
                                                   .ToArrayAsync();


        //for (int i = 0; i < poleAutoru.Length; i++)
        //{
        //    authors[i] = new AuthorDto(poleAutoru[i].Id, poleAutoru[i].Name);
        //}


        return TypedResults.Ok(minuteMeals);
    }

    public static async Task<Results<NotFound, Ok<MinuteMealDto>>> GetMinuteMealById(int id,  MinuteContext context)
    {
        // 📖 3. Vrácení jednoho autora podle id (už je implementováno, jen ho zkontrolujte).

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
            // 🚀 4. Změna autora v databázi.

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
            // 🚀 5. Odstranění autora z databáze.

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
