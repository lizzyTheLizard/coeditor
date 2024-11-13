using System.ComponentModel.DataAnnotations;

namespace CoEditor.Domain.Model;

public class ConversationMessage
{
    public required DateTime PromptedAt { get; init; }

    [StringLength(FieldLengths.PromptMaxLength)]
    public required string Prompt { get; init; }

    public ConversationMessageType Type { get; init; }

    [StringLength(FieldLengths.TextMaxLength)]
    public string? Response { get; init; }

    public long? DurationInMs { get; init; }

    public override string ToString()
    {
        return
            $"{base.ToString()} : Type={Type}, Prompt={Prompt}, PromptedAt={PromptedAt}, Response={Response}, DurationInMs={DurationInMs}";
    }
}
