using CoEditor.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddWebAssemblyClient(builder.HostEnvironment);
await builder.Build().RunAsync();
