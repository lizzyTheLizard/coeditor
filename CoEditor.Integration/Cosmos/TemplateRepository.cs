using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CoEditor.Integration.Cosmos;

internal class TemplateRepository(CosmosDbContext _dbContext) : ITemplateRepository
{
    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var userTemplates = await _dbContext.Templates
            .Where(t => t.Language == language)
            .Where(t => t.UserName == userName)
            .Select(t => new Template { Id = t.Id, Text = t.Text, Name = t.Name })
            .ToArrayAsync();
        var defaultTemplate = GetDefaultTemplates(language);
        return [.. userTemplates, .. defaultTemplate];
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    private static Template[] GetDefaultTemplates(Language language)
    {
        //TODO: Fix for other languages
        return
        [
            new Template { Id = Guid.Empty, Name = "Default", Text = "I want to write {Context:text}" },
            new Template
            {
                Id = Guid.Empty,
                Name = "Email Work Collegue",
                Text =
                    "I want to write an email to a work collegue named {Name:string}. I know him {Knowledge:select:well,barely}. The tone should be {Tone:select:humble,aggressive,frindly}. The content of the mail consideres {Context:text}"
            }
        ];
    }
}
