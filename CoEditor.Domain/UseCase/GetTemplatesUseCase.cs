using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class GetTemplatesUseCase(
    ITemplateRepository templateRepository,
    ILogger<GetTemplatesUseCase> logger) : IGetTemplatesApi
{
    private static readonly SystemTemplate[] SystemTemplates =
    [
        new("Ohne Kontext", string.Empty,
            "Without Context", string.Empty),
        new("Mit Kontext", "{Inhalt:longtext}",
            "With Context", "{Context:longtext}"),
        new("E-Mail",
            "Ich will eine E-Mail an einen {Verbindung:select:Arbeitskollege, Freund, Kunde} namens {Name:text} schreiben. Ich kenne diese Person {Kenntnis:select:Gut, Kaum}. Der Ton des Mails soll {Ton:select:Bescheiden, Aggressiv, Freundlich} sein. Das Mail geht um folgendes:\n\n{Inhalt:longtext}",
            "E-Mail",
            "I want to write an email to {Connection:select:Work Collegue,Friend, Customer} {Name:text}. I know him {Knowledge:select:Well,Barely}. The tone should be {Tone:select:Humble,Aggressive,Friendly}. The content of the mail considers the followig:\n\n{Context:longtext}")
    ];

    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var userTemplates = await templateRepository.GetTemplatesAsync(userName, language);
        Template[] templates = [.. GetSystemTemplates(userName, language), .. userTemplates];
        logger.TemplatesLoaded(templates, userName, language);
        return templates;
    }

    private static Template[] GetSystemTemplates(string userName, Language language)
    {
        return SystemTemplates
            .Select(t => t.ToTemplate(userName, language))
            .ToArray();
    }
}

#pragma warning disable SA1402 // This is a purely internal class and should not be split into multiple files
internal record SystemTemplate(string NameDe, string TextDe, string NameEn, string TextEn)
{
    public Template ToTemplate(string userName, Language language)
    {
        return new Template
        {
            Id = Guid.NewGuid(),
            Name = language switch
            {
                Language.De => NameDe,
                Language.En => NameEn,
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Text = language switch
            {
                Language.De => TextDe,
                Language.En => TextEn,
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            UserName = userName,
            Language = language,
            DefaultTemplate = true
        };
    }
}

#pragma warning disable SA1402,SA1204 // LogMessages are only used in this file
internal static class GetTemplatesLogMessages
{
    public static void TemplatesLoaded(this ILogger logger, Template[] templates, string userName, Language language)
    {
        logger.LogDebug("Loaded {NbrTemplates} templates in {Language} for {UserName}", templates.Length, language,
            userName);
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        foreach (var t in templates) logger.LogTrace("{Template}", t);
    }
}
