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
            .Select(t => new Template
            {
                Id = t.Id,
                Language = t.Language,
                UserName = t.UserName,
                Text = t.Text,
                Name = t.Name,
                DefaultTemplate = false
            })
            .ToArrayAsync();
    }

    public Task DeleteTemplateAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Template?> FindTemplateAsync(Guid id)
    {
        return dbContext.Templates
            .Where(t => t.Id == id)
            .Select(t => new Template
            {
                Id = t.Id,
                Language = t.Language,
                UserName = t.UserName,
                Text = t.Text,
                Name = t.Name,
                DefaultTemplate = false
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Template> UpdateTemplateAsync(Template tmpl)
    {
        var existingDocument = await dbContext.Templates
            .Where(t => t.Id == tmpl.Id)
            .FirstAsync();
        existingDocument.Text = tmpl.Text;
        existingDocument.Name = tmpl.Name;
        await dbContext.SaveChangesAsync();
        return tmpl;
    }

    public async Task<Template> CreateTemplateAsync(Template tmpl)
    {
        var document = new TemplateDocument
        {
            Id = tmpl.Id,
            Language = tmpl.Language,
            Name = tmpl.Name,
            UserName = tmpl.UserName,
            Text = tmpl.Text
        };
        dbContext.Add(document);
        await dbContext.SaveChangesAsync();
        return tmpl;
    }
}
