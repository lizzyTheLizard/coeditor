using System.Globalization;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;

namespace CoEditor.Domain.UseCase;

internal class PromptMessageFactory
{
    private readonly Dictionary<Language, IFormatProvider> _formatProviders = new()
    {
        { Language.De, new CultureInfo("de-De") }, { Language.En, new CultureInfo("en-US") },
    };

    private readonly CommandPrompt[] commandPrompts =
    [
        new(Language.De, ActionName.Improve, true, "Verbessere den gesammten Text"),
        new(Language.En, ActionName.Improve, true, "Improve the whole text"),
        new(Language.De, ActionName.Improve, false, "Verbessere den folgenden Teil und gib nur den verbesserten Text zurück: {0}"),
        new(Language.En, ActionName.Improve, false, "Improve the following part and return only the improved text: {0}"),
        new(Language.De, ActionName.Expand, true, "Erweitere den gesammten Text mit zusätzlichen Erklärungen"),
        new(Language.En, ActionName.Expand, true, "Expand the whole text with additional explanations"),
        new(Language.De, ActionName.Expand, false, "Erweitere den folgenden Teil mit zusätzlichen Erklärungen und gib nur den verbesserten Text zurück: {0}"),
        new(Language.En, ActionName.Expand, false, "Expand the following part with additional explanations and return only the improved text: {0}"),
        new(Language.De, ActionName.Summarize, true, "Fasse den gesammten Text zusammen"),
        new(Language.En, ActionName.Summarize, true, "Summarize the whole text"),
        new(Language.De, ActionName.Summarize, false, "Fasse den folgenden Teil zusammen und gib nur den verbesserten Text zurück: {0}"),
        new(Language.En, ActionName.Summarize, false, "Summarize the following part and return only the improved text: {0}"),
        new(Language.De, ActionName.Reformulate, true, "Formuliere den gesammten Text um"),
        new(Language.En, ActionName.Reformulate, true, "Reformulate the whole text"),
        new(Language.De, ActionName.Reformulate, false, "Formuliere den folgenden Teil um und gib nur den verbesserten Text zurück: {0}"),
        new(Language.En, ActionName.Reformulate, false, "Reformulate the following part and return only the improved text: {0}")
    ];

    private readonly CommandPrompt[] contextChangedMessageTemplates =
    [
        new(Language.De, "Der Kontext hat sich geändert. {0}"),
        new(Language.En, "The context has changed. {0}")
    ];

    private readonly CommandPrompt[] initialCommandTemplates =
    [
        new(Language.De, "Mache einen neuen Vorschlag"),
        new(Language.En, "Create an initial proposal")
    ];

    private readonly CommandPrompt[] initialContextMessageTemplates =
    [
        new(Language.De, "{0}"),
        new(Language.En, "{0}")
    ];

    private readonly CommandPrompt[] systemChatMessageTemplates =
    [
        new(Language.De, "Du bist ein hilfreicher Assistent, der mich beim Schreiben von kurzen Texten unterstützt. Ich schreibe einen Text und du ergänzt ihn für mich auf verschiedene Arten. Gib immer nur die Antwort zurück, aber niemals Zusatzinformationen oder Erklärungen"),
        new(Language.En, "You are a helpful assistant who supports me in writing short texts. I write the text and ask you help me in different ways. Always return the text, but never any additional informations. Do not include any explanation")
    ];

    private readonly CommandPrompt[] textChangedMessageTemplates =
    [
        new(Language.De, "Ich habe den Text in folgendes abgeändert: {0}"),
        new(Language.En, "I have changed the text to the following: {0}")
    ];

    public PromptMessage[] GenerateInitialMessages(Conversation conversation, Profile profile)
    {
        var formatProvider = _formatProviders[conversation.Language];
        var systemChatMessage = systemChatMessageTemplates.First(p => p.Language == conversation.Language);
        var initialContextMessage = initialContextMessageTemplates.First(p => p.Language == conversation.Language);
        var initialContextPrompt = string.Format(formatProvider, initialContextMessage.Prompt, conversation.Context);
        var initialCommand = initialCommandTemplates.First(p => p.Language == conversation.Language);
        return
        [
            new PromptMessage(systemChatMessage.Prompt, PromptMessageType.System),
            new PromptMessage(profile.Text, PromptMessageType.User),
            new PromptMessage(initialContextPrompt, PromptMessageType.User),
            new PromptMessage(initialCommand.Prompt, PromptMessageType.User)
        ];
    }

    public PromptMessage[] GenerateActionMessages(Conversation conversation, HandleActionInput input, string prompt)
    {
        var result = new List<PromptMessage>();
        var formatProvider = _formatProviders[conversation.Language];
        if (conversation.Context != input.NewContext)
        {
            var contextChangedMessage = contextChangedMessageTemplates.First(p => p.Language == conversation.Language);
            var contextChangedPrompt = string.Format(formatProvider, contextChangedMessage.Prompt, input.NewContext);
            result.Add(new PromptMessage(contextChangedPrompt, PromptMessageType.User));
        }

        if (conversation.Text != input.NewText)
        {
            var textChangedMessage = textChangedMessageTemplates.First(p => p.Language == conversation.Language);
            var textChangedPrompt = string.Format(formatProvider, textChangedMessage.Prompt, input.NewText);
            result.Add(new PromptMessage(textChangedPrompt, PromptMessageType.User));
        }

        result.Add(new PromptMessage(prompt, PromptMessageType.User));
        return [.. result];
    }

    public string GetCommandPrompt(HandleNamedActionInput input)
    {
        var formatProvider = _formatProviders[input.Language];
        var prompt = commandPrompts
            .Where(p => p.Language == input.Language)
            .Where(p => p.Name == input.Action)
            .First(p => p.HasFullText == (input.Selection == null));
        if (input.Selection == null)
        {
            return prompt.Prompt;
        }

        var selectionLength = input.Selection.End - input.Selection.Start;
        var selectedText = input.NewText.Substring(input.Selection.Start, selectionLength);
        return string.Format(formatProvider, prompt.Prompt, selectedText);
    }
}

#pragma warning disable SA1402 // This is a purely internal class and should not be split into multiple files
internal record CommandPrompt(Language Language, ActionName? Name, bool? HasFullText, string Prompt)
{
    public CommandPrompt(Language language, string prompt)
        : this(language, null, null, prompt)
    {
    }
}
