namespace CoEditor.Domain.Model;

public static class FieldLengths
{
    public const int NameMaxLength = 100;
    public const int TextMaxLength = 10000;
    public const int ContextMaxLength = TextMaxLength;
    public const int ProfileMaxLength = TextMaxLength;
    public const int PromptMaxLength = TextMaxLength + 1000;
}
