using CoEditor.Server;

var app = WebApplication
    .CreateBuilder(args)
    .AddAuthentication()
    .AddDatabase()
    .AddRazor()
    .AddLog()
    .Build();

app
    .UseLog()
    .UseAuthentication()
    .UseRazor()
    .Run();
