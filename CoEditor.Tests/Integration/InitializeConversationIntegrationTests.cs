using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class InitializeConversationIntegrationTests : IntegrationTestBase
{
    private readonly IInitializeConversationApi _target;

    public InitializeConversationIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _target = GetRequiredService<IInitializeConversationApi>();
    }

    [Fact]
    public async Task WithoutContext_NoInitialCall()
    {
        var input = new InitializeConversationInput
        {
            ConversationGuid = Guid.NewGuid(),
            NewContext = string.Empty,
            NewText = string.Empty,
            Language = Language.En
        };

        var result = await _target.InitializeConversationAsync(input);

        Assert.NotNull(result);
        Assert.Equal(input.ConversationGuid, result.Id);
        Assert.Empty(result.Context);
        Assert.Empty(result.Text);
        Assert.Equal(input.Language, result.Language);
        Assert.Equal(UserName, result.UserName);
        Assert.Single(result.Messages);
    }

    [Fact]
    public async Task WithContext_InitialText()
    {
        var input = new InitializeConversationInput
        {
            ConversationGuid = Guid.NewGuid(),
            NewContext = "A Joke",
            NewText = string.Empty,
            Language = Language.En
        };

        var result = await _target.InitializeConversationAsync(input);

        Assert.NotNull(result);
        Assert.Equal(input.ConversationGuid, result.Id);
        Assert.Equal(input.NewContext, result.Context);
        Assert.NotEmpty(result.Text);
        Assert.Equal(input.Language, result.Language);
        Assert.Equal(UserName, result.UserName);
        Assert.Equal(2, result.Messages.Length);
    }
}
