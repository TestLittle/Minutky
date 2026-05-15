using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using UTB.Minute.CanteenClient.Components;
using UTB.Minute.CanteenClient.Services;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient<CanteenApiClient>(client =>
{
    client.BaseAddress = new Uri("http://webapi");
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddKeycloakOpenIdConnect(
  serviceName: "keycloak",
  realm: "utb-minute",
  options =>
  {
      options.ClientId = "utb-minute-canteenclient";
      options.ClientSecret = "EYMr3eh2Rf6aQNl4HQBYeGoI3nqQzViC"; // jen dev
      options.ResponseType = OpenIdConnectResponseType.Code;
      options.Scope.Add("openid");
      options.Scope.Add("offline_access");
      options.SaveTokens = true;
      options.RequireHttpsMetadata = false; // jen dev
      options.TokenValidationParameters.NameClaimType = "preferred_username";
  });

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
{
    options.RefreshBeforeExpiration = TimeSpan.FromSeconds(30);
});

builder.Services.AddUserAccessTokenHttpClient<CanteenApiClient>(
  configureClient: (_, c) => c.BaseAddress = new Uri("https://webapi"));


var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/logout", async (HttpContext ctx) =>
{
    string? idToken = await ctx.GetTokenAsync("id_token");

    await ctx.RevokeRefreshTokenAsync();

    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/",
        Parameters = { { "id_token_hint", idToken ?? string.Empty } }
    });
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
