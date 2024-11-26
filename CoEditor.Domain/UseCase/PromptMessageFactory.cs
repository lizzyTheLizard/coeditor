using System.Globalization;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;

namespace CoEditor.Domain.UseCase;

internal class PromptMessageFactory
{
    private readonly List<CommandMessageType> _commands =
    [
        new(ActionName.Improve, true,
            "Verbessere den ganzen Text. Korriegiere Rechtschreibfehler und verbessere die Grammatik im ganzen Text. Korrigiere Stilfehler aber ändere nicht die Bedeutung.",
            "Improve the entire text. Correct spelling mistakes and enhance the grammar throughout the text. Correct stylistic errors but do not change the meaning."),
        new(ActionName.Improve, false,
            "Verbessere den folgenden Teil und gib nur den verbesserten Text zurück. Korriegiere Rechtschreibfehler und verbessere die Grammatik im ganzen Text. Korrigiere Stilfehler aber ändere nicht die Bedeutung\n\n{0}",
            "Improve the following part and return only the improved text.Correct spelling mistakes and enhance the grammar throughout the text. Correct stylistic errors but do not change the meaning.\n\n{0}"),
        new(ActionName.Expand, true,
            "Erweitere den gesammten Text mit zusätzlichen Erklärungen",
            "Expand the whole text with additional explanations"),
        new(ActionName.Expand, false,
            "Erweitere den folgenden Teil mit zusätzlichen Erklärungen und gib nur den verbesserten Text zurück\n\n{0}",
            "Expand the following part with additional explanations and return only the improved text\n\n{0}"),
        new(ActionName.Summarize, true,
            "Fasse den gesammten Text zusammen und kürze ihn",
            "Summarize the entire text and shorten it"),
        new(ActionName.Summarize, false,
            "Fasse den folgenden Teil zusammen und gib nur den verbesserten Text zurück\n\n{0}",
            "Summarize the following part and shorten it. Return only the improved text\n\n{0}"),
        new(ActionName.Reformulate, true,
            "Formuliere den gesammten Text um",
            "Reformulate the whole text"),
        new(ActionName.Reformulate, false,
            "Formuliere den folgenden Teil um und gib nur den verbesserten Text zurück\n\n{0}",
            "Reformulate the following part and return only the improved text\n\n{0}")
    ];

    private readonly MessageType _contextChangedMessage = new(
        "{0}",
        "{0}");

    private readonly MessageType _initialMessage = new(
        "Mache einen Vorschlag für einen Text mit dem folgenden Kontext:\n\n{0}",
        "Make a suggestion for a text with the following context:\n\n{0}");

    private readonly MessageType _systemMessage = new(PromptMessageType.System,
        "Du bist ein hilfreicher Assistent, der mich beim Schreiben von kurzen Texten unterstützt. Ich schreibe einen Text und du ergänzt ihn für mich auf verschiedene Arten. Gib immer nur die Antwort zurück, aber niemals Zusatzinformationen oder Erklärungen.Bitte berücksichte die folgenden Informationen über mich:\n\n{0}",
        "You are a helpful assistant who supports me in writing short texts. I write the text and ask you help me in different ways. Always return the text, but never any additional informations. Do not include any explanation. Please respect the following information about me:\n\n{0}");

    private readonly MessageType _textChangedMessage = new(
        "Ich habe bisher den folgenden Text geschrieben:\n\n {0}",
        "I have written the following text so far:\n\n {0}");

    public PromptMessage[] GenerateInitialMessages(Conversation conversation, Profile profile)
    {
        var result = new List<PromptMessage> { _systemMessage.GetPrompt(conversation.Language, profile.Text) };
        if (conversation.Context == string.Empty) return result.ToArray();
        result.Add(_initialMessage.GetPrompt(conversation.Language, conversation.Context));
        return result.ToArray();
    }

    public PromptMessage[] GenerateActionMessages(Conversation conversation, HandleActionInput input)
    {
        var result = new List<PromptMessage>();
        if (conversation.Context != input.NewContext)
            result.Add(_contextChangedMessage.GetPrompt(conversation.Language, input.NewContext));
        if (conversation.Text != input.NewText)
            result.Add(_textChangedMessage.GetPrompt(conversation.Language, input.NewText));
        result.Add(input switch
        {
            HandleNamedActionInput actionInput => GetPromptMessage(actionInput),
            HandleCustomActionInput actionInput => new PromptMessage(actionInput.Action, PromptMessageType.User),
            _ => throw new InvalidOperationException($"Action type {input.GetType()} not supported")
        });
        return result.ToArray();
    }

    private PromptMessage GetPromptMessage(HandleNamedActionInput input)
    {
        var command = _commands
            .Where(p => p.Name == input.Action)
            .First(p => p.FullText == (input.Selection == null));
        if (input.Selection == null) return command.GetPrompt(input.Language);
        var selectionLength = input.Selection.End - input.Selection.Start;
        var selectedText = input.NewText.Substring(input.Selection.Start, selectionLength);
        return command.GetPrompt(input.Language, selectedText);
    }
}

#pragma warning disable SA1402 // This is a purely internal class and should not be split into multiple files
internal record MessageType(string FormatDe, string FormatEn)
{
    private readonly PromptMessageType _type = PromptMessageType.User;

    public MessageType(PromptMessageType type, string formatDe, string formatEn)
        : this(formatDe, formatEn)
    {
        _type = type;
    }

    public PromptMessage GetPrompt(Language language, params object?[] args)
    {
        return language switch
        {
            Language.En => new PromptMessage(string.Format(new CultureInfo("en-US"), FormatEn, args), _type),
            Language.De => new PromptMessage(string.Format(new CultureInfo("de-De"), FormatDe, args), _type),
            _ => throw new InvalidOperationException($"Language {language} not supported")
        };
    }
}

internal record CommandMessageType(ActionName Name, bool FullText, string FormatDe, string FormatEn)
    : MessageType(FormatDe, FormatEn);
