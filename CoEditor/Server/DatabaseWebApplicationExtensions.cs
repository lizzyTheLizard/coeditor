using CoEditor.Data;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Server;

public static class DatabaseWebApplicationExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        return builder;
    }
}
