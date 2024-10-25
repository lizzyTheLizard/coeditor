using CoEditor.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddWebAssemblyClient(builder.HostEnvironment);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
await builder.Build().RunAsync();
