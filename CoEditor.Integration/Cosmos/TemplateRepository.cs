using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class TemplateRepository(CosmosDbContext dbContext) : ITemplateRepository
{
    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        return await dbContext.Templates
            .Where(t => t.Language == language)
            .Where(t => t.UserName == userName)
            .Select(t => new Template { Id = t.Id, Text = t.Text, Name = t.Name })
            .ToArrayAsync();
    }
}
