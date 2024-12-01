using CoEditor.Domain.Dependencies;
using CoEditor.Tests.Helper;

namespace CoEditor.Tests.Domain;

public class ConversationUnitTests
{
    [Fact]
    public void ToPromptMessages_Empty()
    {
        var conversation = TestData.ConversationWithoutMessages("TestUser");
        var promptMessages = conversation.ToPromptMessages();
        Assert.Empty(promptMessages);
    }

    [Fact]
    public void ToPromptMessages_NonEmpty()
    {
        var conversation = TestData.ConversationWithMessages("TestUser");
        var promptMessages = conversation.ToPromptMessages();
        Assert.Equal(4, promptMessages.Length);
        Assert.Equal("System Prompt", promptMessages[0].Prompt);
        Assert.Equal(PromptMessageType.System, promptMessages[0].Type);
        Assert.Equal("System Response", promptMessages[1].Prompt);
        Assert.Equal(PromptMessageType.Assistant, promptMessages[1].Type);
        Assert.Equal("User Prompt", promptMessages[2].Prompt);
        Assert.Equal(PromptMessageType.User, promptMessages[2].Type);
        Assert.Equal("User Response", promptMessages[3].Prompt);
        Assert.Equal(PromptMessageType.Assistant, promptMessages[3].Type);
    }

    [Fact]
    public void UpdateTextAndContext()
    {
        var conversation = TestData.ConversationWithoutMessages("TestUser");
        var updated = conversation.UpdateTextAndContext("new text", "new context");
        Assert.Equal(conversation.Id, updated.Id);
        Assert.Equal(conversation.UserName, updated.UserName);
        Assert.Equal(conversation.StartedAt, updated.StartedAt);
        Assert.Equal(conversation.Language, updated.Language);
        Assert.Equal("new text", updated.Text);
        Assert.Equal("new context", updated.Context);
        Assert.Empty(updated.Messages);
    }

    [Fact]
    public void UpdateMessages()
    {
        var conversation = TestData.ConversationWithoutMessages("TestUser");
        var updated = conversation.UpdateMessages(
            [
                new PromptMessage("System Prompt", PromptMessageType.System),
                new PromptMessage("User Prompt", PromptMessageType.User)
            ],
            new PromptResult("Response", 17));
        var promptMessages = updated.ToPromptMessages();
        Assert.Equal(3, promptMessages.Length);
        Assert.Equal("System Prompt", promptMessages[0].Prompt);
        Assert.Equal(PromptMessageType.System, promptMessages[0].Type);
        Assert.Equal("User Prompt", promptMessages[1].Prompt);
        Assert.Equal(PromptMessageType.User, promptMessages[1].Type);
        Assert.Equal("Response", promptMessages[2].Prompt);
        Assert.Equal(PromptMessageType.Assistant, promptMessages[2].Type);
    }
}
