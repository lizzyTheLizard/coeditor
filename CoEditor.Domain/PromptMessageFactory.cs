using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;

namespace CoEditor.Domain;
internal class PromptMessageFactory
{
    private readonly Dictionary<Language, string> SystemChatMessage = new() {
        { Language.DE, "Du bist ein hilfreicher Assistent der mich beim schreiben von kurzen Texten unterstützt. Ich schreibe einen Text und du ergänzt ihn für mich auf verschiedene Arten. Gib immer den ganzen Text zurück, aber niemals Zusatzinformationen"},
        { Language.EN, "You are a helpful assistant who supports me in writing short texts. I write the text and ask you help me in different ways. Always return the full text, but never any additional informations."}
    };
    private readonly Dictionary<Language, string> InitialContextMessage = new() {
        { Language.DE, "Der Kontext ist folgender: {0}"},
        { Language.EN,"The context is: {0}" }
    };
    private readonly Dictionary<Language, string> ContextChangedMessage = new() {
        { Language.DE, "Der Kontext hat geändert. Es geht nun um {0}"},
        { Language.EN, "The context has changed. It is now about {0}"}
    };
    private readonly Dictionary<Language, string> TextChangedMessage = new() {
        { Language.DE, "Ich habe den Text wie folgt abgeändert: {0}"},
        { Language.EN, "I have changed the text as follows: {0}"}
    };
    private readonly Dictionary<Language, string> InitialCommand = new() {
        { Language.DE, "Mache einen neuen Vorschlag"},
        { Language.EN, "Create a proposal"}
    };
    private readonly CommandPrompt[] CommandPrompts = [
        new CommandPrompt(Language.DE, ActionName.Improve, true, "Verbessere den gesammten Text"),
        new CommandPrompt(Language.EN, ActionName.Improve, true, "Improve the whole text"),
        new CommandPrompt(Language.DE, ActionName.Improve, false, "Verbessere den folgenden Teil und lasse den Rest wie bisher: {0}"),
        new CommandPrompt(Language.EN, ActionName.Improve, false, "Improve the following part and let the rest untouched: {0}"),
        new CommandPrompt(Language.DE, ActionName.Expand, true, "Erweitere den gesammten Text mit zusätzlichen Erklärungen"),
        new CommandPrompt(Language.EN, ActionName.Expand, true, "Expand the whole text with additional explanations"),
        new CommandPrompt(Language.DE, ActionName.Expand, false, "Erweitere den folgenden Teil mit zusätzlichen Erklärungen und lasse den Rest wie bisher: {0}"),
        new CommandPrompt(Language.EN, ActionName.Expand, false, "Expand the following part with additional explanations and let the rest untouched: {0}"),
        new CommandPrompt(Language.DE, ActionName.Summarize, true, "Fasse den gesammten Text zusammen"),
        new CommandPrompt(Language.EN, ActionName.Summarize, true, "Summarize the whole text"),
        new CommandPrompt(Language.DE, ActionName.Summarize, false, "Fasse den folgenden Teil zusammen und lasse den Rest wie bisher: {0}"),
        new CommandPrompt(Language.EN, ActionName.Summarize, false, "Summarize the following part and let the rest untouched: {0}"),
        new CommandPrompt(Language.DE, ActionName.Reformulate, true, "Formuliere den gesammten Text um"),
        new CommandPrompt(Language.EN, ActionName.Reformulate, true, "Reformulate the whole text"),
        new CommandPrompt(Language.DE, ActionName.Reformulate, false, "Formuliere den folgenden Teil und lasse den Rest wie bisher: {0}"),
        new CommandPrompt(Language.EN, ActionName.Reformulate, false, "Reformulate the following part and let the rest untouched: {0}")
    ];

    public PromptMessage[] GenerateInitialMessages(Conversation conversation, Profile profile)
    {
        return [
            new PromptMessage(SystemChatMessage[conversation.Language], PromptMessageType.System),
            new PromptMessage(profile.Text, PromptMessageType.System),
            new PromptMessage(string.Format(InitialContextMessage[conversation.Language], conversation.Context), PromptMessageType.System),
            new PromptMessage(InitialCommand[conversation.Language], PromptMessageType.User)
        ];
    }

    public PromptMessage[] GenerateActionMessages(Conversation conversation, HandleCustomActionInput input)
    {
        var result = new List<PromptMessage>();
        if (conversation.Context != input.NewContext)
        {
            var contextMessage = string.Format(ContextChangedMessage[conversation.Language], input.NewContext);
            result.Add(new PromptMessage(contextMessage, PromptMessageType.System));
        }
        if (conversation.Text != input.NewText)
        {
            var textMessage = string.Format(TextChangedMessage[conversation.Language], input.NewText);
            result.Add(new PromptMessage(textMessage, PromptMessageType.User));
        }
        result.Add(new PromptMessage(input.Action, PromptMessageType.User));
        return [.. result];
    }

    public string GetCommandPrompt(HandleNamedActionInput input)
    {
        var prompt = CommandPrompts
            .Where(p => p.Language == input.Language)
            .Where(p => p.Name == input.Action)
            .Where(p => p.HasFullText == (input.Selection == null))
            .First();
        if (input.Selection == null) return prompt.Prompt;
        var selectionLength = input.Selection.End - input.Selection.Start;
        var selectedText = input.NewText.Substring(input.Selection.Start, selectionLength);
        return string.Format(prompt.Prompt, selectedText);
    }
}

internal record CommandPrompt(Language Language, ActionName Name, bool HasFullText, string Prompt) { }
