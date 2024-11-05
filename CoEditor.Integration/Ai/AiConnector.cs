using System.ClientModel;
using System.Diagnostics;
using Azure.AI.OpenAI;
using CoEditor.Domain.Dependencies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace CoEditor.Integration.Ai;

internal class AiConnector(IOptions<AzureOpenAiConfiguration> optionsProvider, ILogger<AiConnector> logger)
    : IAiConnector
{
    private readonly ChatClient _chatClient = CreateChatClient(optionsProvider.Value);

    public async Task<PromptResult> PromptAsync(PromptMessage[] newMessages)
    {
        logger.PromptStarted(newMessages);
        var chatMessages = ToChatMessages(newMessages);
        var watch = Stopwatch.StartNew();
        var completion = await _chatClient.CompleteChatAsync(chatMessages);
        var response = completion.Value.Content[0].Text;
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        logger.PromptFinished(response, elapsedMs);
        return new PromptResult(response, elapsedMs);
    }

    private static List<ChatMessage> ToChatMessages(PromptMessage[] messages)
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
                default:
                    throw new ArgumentOutOfRangeException($"Unknown message type: {message.Type}");
            }
        }

        return chatMessages;
    }

    private static ChatClient CreateChatClient(AzureOpenAiConfiguration options)
    {
        var uri = new Uri(options.Endpoint);
        var credential = new ApiKeyCredential(options.ApiKey);
        var openApiClient = new AzureOpenAIClient(uri, credential);
        return openApiClient.GetChatClient(options.Model);
    }
}
