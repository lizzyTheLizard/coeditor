using CoEditor.Domain.Incomming;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Domain;

public static class DomainWebApplicationExtensions
{
    public static void AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEditorActionApi, EditorActionService>();
        services.AddScoped<ITemplateApi, TemplateService>();
    }
}
