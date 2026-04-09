using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;
using UTB.Minute.Db;
using System.Net.Http.Json;
using UTB.Minute.Contracts;

namespace UTB.Minute.WebApi.Tests;

public class TestFixture : IAsyncLifetime
{
    private DistributedApplication app = null!;
    private string? connectionString;
    public HttpClient HttpClient { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.UTB_Minute_AppHost>(["--environment=Testing"], TestContext.Current.CancellationToken);

        app = await builder.BuildAsync(TestContext.Current.CancellationToken);

        await app.StartAsync(TestContext.Current.CancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("database", TestContext.Current.CancellationToken);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("webapi", TestContext.Current.CancellationToken);

        connectionString = await app.GetConnectionStringAsync("database", TestContext.Current.CancellationToken);
        HttpClient = app.CreateHttpClient("webapi", "https");

        using var context = CreateContext();

        await context.Database.EnsureDeletedAsync(TestContext.Current.CancellationToken);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);


        MinuteMeal test_mm1 = new MinuteMeal { Desc = "Testovaci rizek", Price = 145.99, IsActive = true };
        MinuteMeal test_mm2 = new MinuteMeal { Desc = "Testovaci Caesar salat", Price = 109.99, IsActive = true };
        MinuteMeal test_mm3 = new MinuteMeal { Desc = "Testovaci rohlik", Price = 2.90, IsActive = false };


        MenuItem test_m1 = new MenuItem { Date = new DateTime(new DateOnly(2026, 3, 17), new TimeOnly(12, 25)), Portions = 12, MinuteMeal = test_mm1 };
        MenuItem test_m2 = new MenuItem { Date = new DateTime(new DateOnly(2026, 3, 17), new TimeOnly(12, 25)), Portions = 15, MinuteMeal = test_mm2 };
        MenuItem test_m3 = new MenuItem { Date = new DateTime(new DateOnly(2026, 3, 17), new TimeOnly(12, 25)), Portions = 20, MinuteMeal = test_mm3 };

        Order test_o1 = new Order { MenuItem = test_m1, OrderStatus = OrderStatus.Cancelled };

        await context.MinuteMeals.AddRangeAsync(test_mm1, test_mm2, test_mm3);
        await context.MenuItems.AddRangeAsync(test_m1, test_m2, test_m3);
        await context.Orders.AddRangeAsync(test_o1);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public MinuteContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MinuteContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new MinuteContext(options);
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        await app.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    [CollectionDefinition("Database collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<TestFixture>
    {
    }

    [Collection("Database collection")]
    public class MealsTests(TestFixture fixture)
    {
        private readonly TestFixture fixture = fixture;

        [Fact]
        public async Task GetAllMinuteMeals_ReturnsAllSeededMinuteMeals()
        {
            var response = await fixture.HttpClient.GetAsync("/minuteMeals", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            MinuteMealDto[]? meals = await response.Content.ReadFromJsonAsync<MinuteMealDto[]>(TestContext.Current.CancellationToken);

            Assert.NotNull(meals);
            Assert.True(meals.Length >= 3);
            Assert.Contains(meals, m => m.Desc == "Testovaci rizek" && m.Price == 145.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci Caesar salat" && m.Price == 109.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci rohlik" && m.Price == 2.90);
        }
    }
}
