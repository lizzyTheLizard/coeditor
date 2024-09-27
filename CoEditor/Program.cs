using CoEditor.Server;

var app = WebApplication
    .CreateBuilder(args)
    .AddAuthentication()
    .AddDatabase()
    .AddRazor()
    .Build();

app
    .UseRazor()
    .UseAuthentication()
    .UseErrorHandling()
    .Run();
