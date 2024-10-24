﻿using System.ComponentModel.DataAnnotations;

namespace CoEditor.Domain.Model;

public class Profile
{
    [StringLength(FieldLengths.ProfileMaxLength)]
    public required string Text { get; init; }

    public required Language Language { get; init; }

    public override string ToString()
    {
        return $"{base.ToString()}: Text={Text}, Language={Language}";
    }
}
