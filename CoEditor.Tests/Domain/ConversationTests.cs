using System.Globalization;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;

namespace CoEditor.Tests.Domain;

public class ConversationTests
{
    [Fact]
    public void ToPromptMessages_Empty()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            StartedAt = DateTime.Now,
            Language = Language.En,
            Text = "Initial Text",
            Context = "Initial Context",
            Messages = []
        };
        var promptMessages = conversation.ToPromptMessages();
        Assert.Empty(promptMessages);
    }

    [Fact]
    public void ToPromptMessages_NonEmpty()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            StartedAt = DateTime.Now,
            Language = Language.En,
            Text = "Initial Text",
            Context = "Initial Context",
            Messages =
            [
                new ConversationMessage
                {
                    PromptedAt = DateTime.Now,
                    Type = ConversationMessageType.System,
                    Prompt = "System Prompt",
                    Response = "System Response"
                },
                new ConversationMessage
                {
                    PromptedAt = DateTime.Now,
                    Type = ConversationMessageType.User,
                    Prompt = "User Prompt",
                    Response = "User Response"
                }
            ]
        };
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
    public void ToString_ContainsEverything()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            StartedAt = DateTime.Now,
            Language = Language.En,
            Text = "Initial Text",
            Context = "Initial Context",
            Messages = []
        };
        var str = conversation.ToString();
        Assert.Contains(conversation.Id.ToString(), str);
        Assert.Contains(conversation.UserName, str);
        Assert.Contains(conversation.StartedAt.ToString(CultureInfo.CurrentCulture), str);
        Assert.Contains(conversation.Language.ToString(), str);
        Assert.Contains(conversation.Text, str);
        Assert.Contains(conversation.Context, str);
    }

    [Fact]
    public void Update()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            StartedAt = DateTime.Now,
            Language = Language.En,
            Text = "Initial Text",
            Context = "Initial Context",
            Messages = []
        };
        var updated = conversation.Update(new HandleActionInput
        {
            ConversationGuid = conversation.Id,
            NewText = "new text",
            NewContext = "new context"
        });
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
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            StartedAt = DateTime.Now,
            Language = Language.En,
            Text = "Initial Text",
            Context = "Initial Context",
            Messages = []
        };
        var updated = conversation.Update(
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
