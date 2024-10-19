using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;

namespace CoEditor.Domain;

internal class EditorActionService(
    IProfileRepository _profileRepository,
    IAiConnector _aiConnector,
    IConversationRepository _conversationRepository) : IEditorActionApi
{
    //TODO: Store this as properties
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

    public async Task<string> InitializeConversationAsync(string userName, InitializeConversationInput input)
    {
        await _conversationRepository.EnsureNotExistingAsync(input.ConversationGuid);
        var conversation = CreateInitialConversation(userName, input);
        var messages = await ToInitialMessages(userName, input);
        var result = await _aiConnector.PromptAsync(messages);
        var updatedConversation = MergeConversation(conversation, messages, result, "", input.Context);
        await _conversationRepository.CreateAsync(updatedConversation);
        return updatedConversation.Text;
    }

    private static Conversation CreateInitialConversation(string userName, InitializeConversationInput input)
    {
        return new Conversation()
        {
            Guid = input.ConversationGuid,
            UserName = userName,
            StartedAt = DateTime.Now,
            Language = input.Language,
            Text = "",
            Context = input.Context,
            Messages = []
        };
    }

    private async Task<PromptMessage[]> ToInitialMessages(string userName, InitializeConversationInput input)
    {
        var profile = await _profileRepository.GetProfileAsync(userName, input.Language);
        return [
            new PromptMessage(SystemChatMessage[input.Language], PromptMessageType.System),
            new PromptMessage(profile.Text, PromptMessageType.System),
            new PromptMessage(string.Format(InitialContextMessage[input.Language], input.Context), PromptMessageType.System),
            new PromptMessage(InitialCommand[input.Language], PromptMessageType.User)
        ];
    }

    private static Conversation MergeConversation(Conversation conversation, PromptMessage[] messages, PromptResult result, string currentText, string context)
    {
        var newMessagesWithoutResponse = messages.Take(messages.Length - 1).Select(m => new ConversationMessage()
        {
            PromtedAt = DateTime.Now,
            Prompt = m.Prompt,
            Type = m.Type == PromptMessageType.User ? ConversationMessageType.User : ConversationMessageType.System,
        });
        var lastMessage = messages.Last();
        var lastMessageWithResponse = new ConversationMessage()
        {
            PromtedAt = DateTime.Now,
            Prompt = lastMessage.Prompt,
            Type = lastMessage.Type == PromptMessageType.User ? ConversationMessageType.User : ConversationMessageType.System,
            Response = result.Response,
            DurationInMs = result.durationInMs,
            Exception = result.exception?.Message,
            StackTrace = result.exception?.StackTrace,
        };
        return new Conversation()
        {
            Guid = conversation.Guid,
            UserName = conversation.UserName,
            StartedAt = conversation.StartedAt,
            Language = conversation.Language,
            Text = result.Response ?? currentText,
            Context = context,
            Messages = [.. conversation.Messages, .. newMessagesWithoutResponse, lastMessageWithResponse]
        };
    }

    public Task<string> HandleEditorCommandAsync(string userName, HandleEditorCommandInput input)
    {
        var commandPrompt = GetCommandPrompt(input);
        var customInput = new HandleCustomEditorCommandInput(input.ConversationGuid, input.Language, commandPrompt, input.Context, input.CurrentText, input.Selection);
        return HandleCustomEditorCommandAsync(userName, customInput);
    }

    private string GetCommandPrompt(HandleEditorCommandInput input)
    {
        var prompt = CommandPrompts
            .Where(p => p.Language == input.Language)
            .Where(p => p.Name == input.Action)
            .Where(p => p.FullText == (input.Selection == null))
            .First();
        if (input.Selection == null) return prompt.Prompt;
        var selectionLength = input.Selection.End - input.Selection.Start;
        var selectedText = input.CurrentText.Substring(input.Selection.Start, selectionLength);
        return string.Format(prompt.Prompt, selectedText);
    }

    public async Task<string> HandleCustomEditorCommandAsync(string userName, HandleCustomEditorCommandInput input)
    {
        var existingConversation = await _conversationRepository.GetAsync(input.ConversationGuid);
        if (existingConversation.UserName != userName) throw new EditorActionException("Wrong username");
        var existingMessages = ToMessages(existingConversation);
        var newMessages = ToNewMessages(existingConversation, input);
        var result = await _aiConnector.PromptAsync([.. existingMessages, ..newMessages]);
        var updatedConversation = MergeConversation(existingConversation, newMessages, result, input.CurrentText, input.Context);
        await _conversationRepository.UpdateAsync(updatedConversation);
        return updatedConversation.Text;
    }

    private static PromptMessage[] ToMessages(Conversation conversation)
    {
        var result = new List<PromptMessage>();
        foreach( var conversationMessage in conversation.Messages)
        {
            var type = conversationMessage.Type == ConversationMessageType.System ? PromptMessageType.System : PromptMessageType.User;
            result.Add(new PromptMessage(conversationMessage.Prompt, type));
            if (conversationMessage.Response != null)
                result.Add(new PromptMessage(conversationMessage.Response, PromptMessageType.Assistant));
        }
        return [.. result];
    }

    private PromptMessage[] ToNewMessages(Conversation conversation, HandleCustomEditorCommandInput input)
    {
        var result = new List<PromptMessage>();
        if (conversation.Context != input.Context)
        {
            var contextMessage = string.Format(ContextChangedMessage[input.Language], input.Context);
            result.Add(new PromptMessage(contextMessage, PromptMessageType.System));
        }
        if (conversation.Text != input.CurrentText)
        {
            var textMessage = string.Format(TextChangedMessage[input.Language], input.CurrentText);
            result.Add(new PromptMessage(textMessage, PromptMessageType.User));
        }
        result.Add(new PromptMessage(input.CustomCommand, PromptMessageType.User));
        return [.. result];
    }
}

internal record CommandPrompt(Language Language, ActionName Name, bool FullText, string Prompt) { }
