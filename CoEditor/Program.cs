using CoEditor.Server;

var app = WebApplication
    .CreateBuilder(args)
    .AddAuthentication()
    .AddDatabase()
    .AddRazor()
    .AddReverseProxyConfig()
    .Build();

app
    .UseReverseProxy()
    .UseAuthentication()
    .UseErrorHandling()
    .UseRazor()
    .Run();
