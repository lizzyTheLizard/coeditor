using Azure.AI.OpenAI;
using CoEditor.Domain.Outgoing;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;
using System.Diagnostics;

namespace CoEditor.Integration.Ai;

internal class AiConnector : IAiConnector
{
    private readonly ChatClient _chatClient;

    public AiConnector(IOptions<AzureOpenAiConfiguration> optionsProvider)
    {
        var options = optionsProvider.Value;
        var uri = new Uri(options.Endpoint);
        var credential = new ApiKeyCredential(options.ApiKey);
        var openApiClient = new AzureOpenAIClient(uri, credential);
        _chatClient = openApiClient.GetChatClient(options.Model);
    }

    public async Task<PromptResult> PromptAsync(PromptMessage[] messages)
    {
        var chatMessages = ToChatMessages(messages);
        var watch = Stopwatch.StartNew();
        string? response = null;
        Exception? exception = null;
        try
        {
            var completion = await _chatClient.CompleteChatAsync(chatMessages);
            response = completion.Value.Content[0].Text;
        }
        catch (Exception e) { exception = e; }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        return new PromptResult(response, exception, elapsedMs);
    }

    private static IEnumerable<ChatMessage> ToChatMessages(PromptMessage[] messages)
    {
        var chatMessages = new List<ChatMessage>();
        foreach (var message in messages)
        {
            switch (message.Type)
            {
                case PromptMessageType.System:
                    chatMessages.Add(new SystemChatMessage(message.Prompt));
                    break;
                case PromptMessageType.User:
                    chatMessages.Add(new UserChatMessage(message.Prompt));
                    break;
                case PromptMessageType.Assistant:
                    chatMessages.Add(new AssistantChatMessage(message.Prompt));
                    break;
            }
        }
        return chatMessages;
    }
}
