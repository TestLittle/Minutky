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


    
    await context.SaveChangesAsync();
});


app.UseHttpsRedirection();

app.Run();
