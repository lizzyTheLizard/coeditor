namespace CoEditor.Domain.Api;

public interface IDeleteTemplateApi
{
    Task DeleteTemplateAsync(Guid templateId);
}
