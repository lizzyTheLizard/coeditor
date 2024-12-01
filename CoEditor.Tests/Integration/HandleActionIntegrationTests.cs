using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using CoEditor.Integration.Cosmos;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class HandleActionIntegrationTests : IntegrationTestBase
{
    private readonly IInitializeConversationApi _initializeConversation;
    private readonly IConversationRepository _repository;
    private readonly IHandleActionApi _target;
    private Conversation? _existingConversation;

    public HandleActionIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _repository = GetRequiredService<IConversationRepository>();
        _target = GetRequiredService<IHandleActionApi>();
        _initializeConversation = GetRequiredService<IInitializeConversationApi>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _repository.DeleteAllConversationsAsync(UserName);
        var initializeInput = new InitializeConversationInput
        {
            ConversationGuid = Guid.NewGuid(),
            NewContext = string.Empty,
            NewText = string.Empty,
            Language = Language.En
        };
        _existingConversation = await _initializeConversation.InitializeConversationAsync(initializeInput);
    }

    [Fact]
    public async Task NoConversation_Error()
    {
        var input = new HandleNamedActionInput
        {
            ConversationGuid = Guid.NewGuid(),
            NewContext = "context",
            NewText = "text",
            Action = ActionName.Improve,
            Language = Language.En
        };

        await Assert.ThrowsAsync<CosmosException>(() => _target.HandleActionAsync(input));
    }

    [Fact]
    public async Task NormalAction_SingleMessage()
    {
        var input = new HandleNamedActionInput
        {
            ConversationGuid = _existingConversation!.Id,
            NewContext = _existingConversation.Context,
            NewText = _existingConversation.Text,
            Action = ActionName.Improve,
            Language = Language.En
        };

        var result = await _target.HandleActionAsync(input);

        Assert.Equal(_existingConversation.Id, result.Id);
        Assert.Equal(_existingConversation.Context, result.Context);
        Assert.NotEqual(_existingConversation.Text, result.Text);
        Assert.Equal(2, result.Messages.Length);
        Assert.Equal(result.Text, result.Messages[1].Response);
        Assert.Equal(ConversationMessageType.User, result.Messages[1].Type);
    }

    [Fact]
    public async Task ContextChanged_MessageAdded()
    {
        var input = new HandleNamedActionInput
        {
            ConversationGuid = _existingConversation!.Id,
            NewContext = "Changed Context",
            NewText = _existingConversation.Text,
            Action = ActionName.Improve,
            Language = Language.En
        };

        var result = await _target.HandleActionAsync(input);

        Assert.Equal(input.NewContext, result.Context);
        Assert.Equal(3, result.Messages.Length);
        Assert.EndsWith(input.NewContext, result.Messages[1].Prompt);
    }

    [Fact]
    public async Task TextChanged_MessageAdded()
    {
        var input = new HandleNamedActionInput
        {
            ConversationGuid = _existingConversation!.Id,
            NewContext = _existingConversation.Context,
            NewText = "New Text",
            Action = ActionName.Improve,
            Language = Language.En
        };

        var result = await _target.HandleActionAsync(input);

        Assert.Equal(input.NewContext, result.Context);
        Assert.Equal(3, result.Messages.Length);
        Assert.EndsWith(input.NewText, result.Messages[1].Prompt);
    }

    [Fact]
    public async Task Selection_OnlyChangeSelection()
    {
        var input = new HandleNamedActionInput
        {
            ConversationGuid = _existingConversation!.Id,
            NewContext = _existingConversation.Context,
            NewText = "This is a wring text with other wring stuff",
            Action = ActionName.Improve,
            Language = Language.En,
            Selection = new Selection(10, 15)
        };

        var result = await _target.HandleActionAsync(input);

        Assert.Equal(input.NewContext, result.Context);
        Assert.Equal("This is a wrong text with other wring stuff", result.Text);
    }

    [Fact]
    public async Task CustomAction_Works()
    {
        var input = new HandleCustomActionInput
        {
            ConversationGuid = _existingConversation!.Id,
            NewContext = _existingConversation.Context,
            NewText = "My name is John.",
            Action = "Translate to german"
        };

        var result = await _target.HandleActionAsync(input);

        Assert.Equal(input.NewContext, result.Context);
        Assert.Equal("Mein Name ist John.", result.Text);
    }
}
