using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CoEditor.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddWebAssemblyClient(builder.Configuration, builder.HostEnvironment);
await builder.Build().RunAsync();
