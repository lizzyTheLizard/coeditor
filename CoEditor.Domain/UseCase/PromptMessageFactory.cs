﻿using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using System.Globalization;

namespace CoEditor.Domain.UseCase;

internal class PromptMessageFactory
{
    private readonly Dictionary<Language, IFormatProvider> _formatProviders = new()
    {
        { Language.De, new CultureInfo("de-De") }, { Language.En, new CultureInfo("en-US") }
    };

    private readonly CommandPrompt[] CommandPrompts =
    [
        new(Language.De, ActionName.Improve, true, "Verbessere den gesammten Text"),
        new(Language.En, ActionName.Improve, true, "Improve the whole text"),
        new(Language.De, ActionName.Improve, false, "Verbessere den folgenden Teil und lasse den Rest wie bisher: {0}"),
        new(Language.En, ActionName.Improve, false, "Improve the following part and let the rest untouched: {0}"),
        new(Language.De, ActionName.Expand, true, "Erweitere den gesammten Text mit zusätzlichen Erklärungen"),
        new(Language.En, ActionName.Expand, true, "Expand the whole text with additional explanations"),
        new(Language.De, ActionName.Expand, false,
            "Erweitere den folgenden Teil mit zusätzlichen Erklärungen und lasse den Rest wie bisher: {0}"),
        new(Language.En, ActionName.Expand, false,
            "Expand the following part with additional explanations and let the rest untouched: {0}"),
        new(Language.De, ActionName.Summarize, true, "Fasse den gesammten Text zusammen"),
        new(Language.En, ActionName.Summarize, true, "Summarize the whole text"),
        new(Language.De, ActionName.Summarize, false,
            "Fasse den folgenden Teil zusammen und lasse den Rest wie bisher: {0}"),
        new(Language.En, ActionName.Summarize, false, "Summarize the following part and let the rest untouched: {0}"),
        new(Language.De, ActionName.Reformulate, true, "Formuliere den gesammten Text um"),
        new(Language.En, ActionName.Reformulate, true, "Reformulate the whole text"),
        new(Language.De, ActionName.Reformulate, false,
            "Formuliere den folgenden Teil und lasse den Rest wie bisher: {0}"),
        new(Language.En, ActionName.Reformulate, false,
            "Reformulate the following part and let the rest untouched: {0}")
    ];

    private readonly CommandPrompt[] ContextChangedMessage =
    [
        new(Language.De, "Der Kontext hat geändert. Es geht nun um {0}"),
        new(Language.En, "The context has changed. It is now about {0}")
    ];

    private readonly CommandPrompt[] InitialCommand =
    [
        new(Language.De, "Mache einen neuen Vorschlag"),
        new(Language.En, "Create a proposal")
    ];

    private readonly CommandPrompt[] InitialContextMessage =
    [
        new(Language.De, "Der Kontext ist folgender: {0}"),
        new(Language.En, "The context is: {0}")
    ];

    private readonly CommandPrompt[] SystemChatMessage =
    [
        new(Language.De,
            "Du bist ein hilfreicher Assistent der mich beim schreiben von kurzen Texten unterstützt. Ich schreibe einen Text und du ergänzt ihn für mich auf verschiedene Arten. Gib immer den ganzen Text zurück, aber niemals Zusatzinformationen"),
        new(Language.En,
            "You are a helpful assistant who supports me in writing short texts. I write the text and ask you help me in different ways. Always return the full text, but never any additional informations.")
    ];

    private readonly CommandPrompt[] TextChangedMessage =
    [
        new(Language.De, "Ich habe den Text wie folgt abgeändert: {0}"),
        new(Language.En, "I have changed the text as follows: {0}")
    ];

    public PromptMessage[] GenerateInitialMessages(Conversation conversation, Profile profile)
    {
        var formatProvider = _formatProviders[conversation.Language];
        var systemChatMessage = SystemChatMessage.First(p => p.Language == conversation.Language);
        var initialContextMessage = InitialContextMessage.First(p => p.Language == conversation.Language);
        var initialContextPrompt = string.Format(formatProvider, initialContextMessage.Prompt, conversation.Context);
        var initialCommand = InitialCommand.First(p => p.Language == conversation.Language);
        return
        [
            new PromptMessage(systemChatMessage.Prompt, PromptMessageType.System),
            new PromptMessage(profile.Text, PromptMessageType.System),
            new PromptMessage(initialContextPrompt, PromptMessageType.System),
            new PromptMessage(initialCommand.Prompt, PromptMessageType.User)
        ];
    }

    public PromptMessage[] GenerateActionMessages(Conversation conversation, HandleCustomActionInput input)
    {
        var result = new List<PromptMessage>();
        var formatProvider = _formatProviders[conversation.Language];
        if (conversation.Context != input.NewContext)
        {
            var contextChangedMessage = ContextChangedMessage.First(p => p.Language == conversation.Language);
            var contextChangedPrompt = string.Format(formatProvider, contextChangedMessage.Prompt, input.NewContext);
            result.Add(new PromptMessage(contextChangedPrompt, PromptMessageType.System));
        }

        if (conversation.Text != input.NewText)
        {
            var textChangedMessage = TextChangedMessage.First(p => p.Language == conversation.Language);
            var textChangedPrompt = string.Format(formatProvider, textChangedMessage.Prompt, input.NewText);
            result.Add(new PromptMessage(textChangedPrompt, PromptMessageType.User));
        }

        result.Add(new PromptMessage(input.Action, PromptMessageType.User));
        return [.. result];
    }

    public string GetCommandPrompt(HandleNamedActionInput input)
    {
        var formatProvider = _formatProviders[input.Language];
        var prompt = CommandPrompts
            .Where(p => p.Language == input.Language)
            .Where(p => p.Name == input.Action)
            .First(p => p.HasFullText == (input.Selection == null));
        if (input.Selection == null) return prompt.Prompt;
        var selectionLength = input.Selection.End - input.Selection.Start;
        var selectedText = input.NewText.Substring(input.Selection.Start, selectionLength);
        return string.Format(formatProvider, prompt.Prompt, selectedText);
    }
}

internal record CommandPrompt(Language Language, ActionName? Name, bool? HasFullText, string Prompt)
{
    public CommandPrompt(Language language, string prompt) : this(language, null, null, prompt) { }
}
