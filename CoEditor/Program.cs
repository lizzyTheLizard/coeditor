using CoEditor.Client;
using CoEditor.Components;
using CoEditor.Domain;
using CoEditor.Integration.Ai;
using CoEditor.Integration.Cosmos;
using CoEditor.Integration.Identity;
using CoEditor.Integration.Insights;
using Microsoft.AspNetCore.HttpOverrides;
using _Imports = CoEditor.Client._Imports;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServerIneractiveClient();
builder.Services.AddDomain();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIServerIdentity(builder.Configuration);
builder.Services.AddAi(builder.Configuration);
builder.Services.AddInsights(builder.Configuration, builder.Environment);
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();
// This is only needed when the database has changed...
//await app.Services.RecreateDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
    app.UseExceptionHandler("/Error", true);

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedHost });
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);
app.Run();
