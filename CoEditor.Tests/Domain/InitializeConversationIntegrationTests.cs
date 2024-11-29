using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using CoEditor.Tests.Helper;

namespace CoEditor.Tests.Domain;

public class InitializeConversationIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task WithoutContext_NoInitialCall()
    {
        var input = new InitializeConversationInput
        {
            ConversationGuid = Guid.NewGuid(),
            NewContext = "",
            NewText = "",
            Language = Language.En
        };

        var result = await factory
            .Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<IInitializeConversationApi>()
            .InitializeConversationAsync(input);

        Assert.NotNull(result);
        Assert.Equal(input.ConversationGuid, result.Id);
        Assert.Equal("", result.Context);
        Assert.Equal("", result.Text);
        Assert.Equal(input.Language, result.Language);
        Assert.Equal("TestUser", result.UserName);
        Assert.Equal(1, result.Messages.Length);
    }
}
