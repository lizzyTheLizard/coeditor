using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class GetTemplatesUseCase(
    ITemplateRepository templateRepository,
    ILogger<GetTemplatesUseCase> logger) : IGetTemplatesApi
{
    private static readonly SystemTemplate DefaultEn = new(
        Guid.Parse("8a93cccb-5cad-4bc7-97ae-78fc7f1da785"),
        "Default",
        "I want to write {Context:LongText}");

    private static readonly SystemTemplate EmailEn = new(
        Guid.Parse("c7dfe23f-250d-40a5-86c8-a3f6debe15f2"),
        "E-Mail",
        "I want to write an email to {Connection:select:Work Collegue,Friend, Customer} {Name:text}. I know him {Knowledge:select:Well,Barely}. The tone should be {Tone:select:Humble,Aggressive,Friendly}. The content of the mail considers {Context:longtext}");

    private static readonly SystemTemplate DefaultDe = new(
        Guid.Parse("fc93a8c5-faeb-4b7b-8064-7c79ba29d65e"),
        "Text",
        "Ich will folgendes schreiben: {Inhalt:LongText}");

    private static readonly SystemTemplate EmailDe = new(
        Guid.Parse("944dee80-cb27-41a0-bfea-37142805edef"),
        "E-Mail",
        "Ich will eine E-Mail an einen {Verbindung:select:Arbeitskollege, Freund, Kunde} namens {Name:text} schreiben. Ich kenne diese Person {Kenntnis:select:Gut, Kaum}. Der Ton des Mails soll {Ton:select:Bescheiden, Aggressiv, Freundlich} sein. Das Mail geht um folgendes: {Inhalt:longtext}");

    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var userTemplates = await templateRepository.GetTemplatesAsync(userName, language);
        Template[] templates = [.. GetSystemTemplates(userName, language), .. userTemplates];
        logger.TemplatesLoaded(templates, userName, language);
        return templates;
    }

    private static Template[] GetSystemTemplates(string userName, Language language)
    {
        SystemTemplate[] systemTemplates = language switch
        {
            Language.De => [DefaultDe, EmailDe],
            Language.En => [DefaultEn, EmailEn],
            _ => throw new NotImplementedException(),
        };
        return systemTemplates
            .Select(t => ToTemplate(t, userName, language))
            .ToArray();
    }

    private static Template ToTemplate(SystemTemplate t, string userName, Language language) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Text = t.Text,
        UserName = userName,
        Language = language,
        DefaultTemplate = true,
    };
}

#pragma warning disable SA1402 // This is a purely internal class and should not be split into multiple files
internal record SystemTemplate(Guid Id, string Name, string Text);

#pragma warning disable SA1402,SA1204 // LogMessages are only used in this file
internal static partial class GetTemplatesLogMessages
{
    public static void TemplatesLoaded(this ILogger logger, Template[] templates, string userName, Language language)
    {
        logger.LogDebug("Loaded {NbrTemplates} templates in {Language} for {UserName}", templates.Length, language, userName);
        if (!logger.IsEnabled(LogLevel.Trace))
        {
            return;
        }

        foreach (var t in templates)
        {
            logger.LogTrace("{Template}", t);
        }
    }
}
