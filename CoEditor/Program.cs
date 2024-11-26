using CoEditor.Components;
using CoEditor.Domain;
using CoEditor.Integration.Ai;
using CoEditor.Integration.Cosmos;
using CoEditor.Integration.Identity;
using CoEditor.Integration.Insights;
using CoEditor.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDomain();
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAi(builder.Configuration);
builder.Services.AddInsights(builder.Configuration, builder.Environment);
builder.Services.AddScoped<UndoService>();
builder.Services.AddScoped<ShortcutService>();
builder.Services.AddScoped<ExceptionService>();
builder.Services.AddScoped<BusyService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error", true);
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedHost });
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
await app.RunAsync();
