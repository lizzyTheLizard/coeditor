using CoEditor.Client;
using CoEditor.Components;
using CoEditor.Domain;
using CoEditor.Integration.Ai;
using CoEditor.Integration.Cosmos;
using CoEditor.Integration.Identity;

using CoEditor.Integration.Insights;
using CoEditor.Rest;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServerInteractiveClient();
builder.Services.AddDomain();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddServerIdentity(builder.Configuration);
builder.Services.AddAi(builder.Configuration);
builder.Services.AddInsights(builder.Configuration, builder.Environment);
builder.Services.AddRest();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", true);
}

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedHost });
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClientWebApplicationExtensions).Assembly);
await app.RunAsync();
