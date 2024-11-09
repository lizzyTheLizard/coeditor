using System.ComponentModel.DataAnnotations;

namespace CoEditor.Domain.Model;

public class Profile
{
    [StringLength(FieldLengths.ProfileMaxLength)]
    public required string UserName { get; init; }

    [StringLength(FieldLengths.ProfileMaxLength)]
    public required string Text { get; set; }

    public required Language Language { get; init; }

    public override string ToString()
    {
        return $"{base.ToString()}: Text={Text}, Language={Language}";
    }
}
