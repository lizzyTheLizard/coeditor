﻿using System.ComponentModel;

namespace CoEditor.Domain.Model;

public enum Language
{
    [Description("Deutsch")]
    De,

    [Description("English")]
    En
}

#pragma warning disable SA1649 // Additional class as it is part of the enum
public static class LanguageExtensions
{
    public static string GetDescription(this Language language)
    {
        var field = language.GetType().GetField(language.ToString()) ??
                    throw new InvalidOperationException($"Language {language} not found");
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        if (attribute == null) return language.ToString();
        var descriptionAttribute = (DescriptionAttribute)attribute;
        return descriptionAttribute.Description;
    }
}
