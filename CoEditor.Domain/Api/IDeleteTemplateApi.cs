namespace CoEditor.Domain.Api;

public interface IDeleteTemplateApi
{
    Task DeleteTemplateAsync(string userName, Guid templateId);
}
