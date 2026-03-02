//using UTB.Minute.Contracts;
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
    public static async Task<Created<AuthorDto>> CreateMinuteMeal(AuthorDto authorDto, MinuteContext context)
    {
        // 🚀 1. Přidání nového autora do databáze.

        MinuteMeal a = new() { Desc = authorDto.Desc };
        context.MinuteMeals.Add(a);
        await context.SaveChangesAsync();

        AuthorDto aDto = new AuthorDto(a.Id, a.Desc);

        return TypedResults.Created($"/authors/{aDto.Id}", aDto);
    }

    public static async Task<Ok<AuthorDto[]>> GetMinuteMeals(MinuteContext context)
    {
        // 🚀 2.Vrácení všech autorů z databáze.
        //2.1 Načíst autory z databáze
        //2.2 Převést je na typ AuthorDto

        MinuteMeal[] poleAutoru = await context.MinuteMeals.ToArrayAsync();

        //AuthorDto[] authors = new AuthorDto[poleAutoru.Length];

        AuthorDto[] authors = await context.MinuteMeals.Where(a => a.Desc.ToLower().Contains("am"))
                                                   .OrderBy(a => a.Desc)
                                                   .Select(a => new AuthorDto(a.Id, a.Name))
                                                   .ToArrayAsync();


        //for (int i = 0; i < poleAutoru.Length; i++)
        //{
        //    authors[i] = new AuthorDto(poleAutoru[i].Id, poleAutoru[i].Name);
        //}


        return TypedResults.Ok(authors);
    }

    public static async Task<Results<NotFound, Ok<AuthorDto>>> GetMinuteMealById(int id,  MinuteContext context)
    {
        // 📖 3. Vrácení jednoho autora podle id (už je implementováno, jen ho zkontrolujte).

        if (await context.Authors.FindAsync(id) is Author author)
        {
            AuthorDto authorDto = new(author.Id, author.Name);

            return TypedResults.Ok(authorDto);
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> UpdateMinuteMeal(int id, AuthorDto authorDto, MinuteContext context)
    {
        if (await context.Authors.FindAsync(id) is Author author)
        {
            // 🚀 4. Změna autora v databázi.

            author.Name = authorDto.Name;

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
        if (await context.Authors.FindAsync(id) is Author author)
        {
            // 🚀 5. Odstranění autora z databáze.

            context.Authors.Remove(author);

            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }
}
