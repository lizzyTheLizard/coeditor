using CoEditor.Domain.Model;

namespace CoEditor.Tests.Helper;

public static class TestData
{
    public static Template TemplateWithoutParameters(string userName) => new()
    {
        Id = Guid.NewGuid(),
        Language = Language.En,
        Text = "TestText",
        UserName = userName,
        Name = "TestName",
        DefaultTemplate = false
    };

    public static Template TemplateWithParameters(string userName) => new()
    {
        Id = Guid.NewGuid(),
        Language = Language.En,
        Text = "{name:text} {long:longtext} {select:select:option1,option2}",
        UserName = userName,
        Name = "TestName2",
        DefaultTemplate = false
    };

    public static Profile Profile(string userName) => new()
    {
        UserName = userName,
        Language = Language.De,
        Text = "TestText"
    };

    public static Conversation ConversationWithoutMessages(string userName) => new()
    {
        Id = Guid.NewGuid(),
        UserName = userName,
        StartedAt = DateTime.Now,
        Language = Language.En,
        Text = "Initial Text",
        Context = "Initial Context",
        Messages = []
    };

    public static Conversation ConversationWithMessages(string userName) => new()
    {
        Id = Guid.NewGuid(),
        UserName = userName,
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
}
