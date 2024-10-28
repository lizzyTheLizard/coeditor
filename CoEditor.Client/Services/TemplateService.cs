using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

//TODO: Clean up the servies => What shall be here and what shall be in component?
public class TemplateService(
    ConversationService conversationService,
    IGetTemplatesApi getTemplatesApi,
    IUpdateTemplateApi updateTemplateApi,
    IDeleteTemplateApi deleteTemplateApi,
    AuthenticationStateProvider authenticationStateProvider,
    ILogger<TemplateService> logger)
{
    private Language _language;
    private Guid _templateId;
    public Template[] Templates { get; private set; } = [];
    public Template Current => Templates.First(t => t.Id == _templateId);
    public TemplateParameter[] TemplateParameters { get; private set; } = [];

    public async Task SetLanguageAsync(Language language)
    {
        _language = language;
        logger.TemplateLanguageChanged(language);
        await LoadTemplatesAsync();
        await TemplateIdChangedAsync(Templates[0].Id);
    }

    public async Task LoadTemplatesAsync()
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            Templates = await getTemplatesApi.GetTemplatesAsync(userName, _language);
            logger.TemplatesLoaded(_language, Templates);
        }
        catch (Exception e)
        {
            Templates = [];
            //TODO: Error Handling: Show error to user!
            logger.TemplatesLoadedFailed(e);
        }
    }

    public async Task TemplateIdChangedAsync(Guid templateId)
    {
        _templateId = templateId;
        logger.TemplateChanged(Current);
        conversationService.EndConversation();
        LoadTemplateParameters();
        await ParameterChangedAsync();
    }

    private void LoadTemplateParameters()
    {
        try
        {
            TemplateParameters = Current.GetTemplateParameters();
        }
        catch (Exception e)
        {
            //TODO: Error Handling: Show error to user!
            logger.TemplateParametersInvalid(e, Current);
            TemplateParameters = [];
        }
    }

    public async Task ParameterChangedAsync()
    {
        var valid = TemplateParameters.All(p => p.Valid);
        if (!valid)
        {
            logger.TemplateParametersNotValid(TemplateParameters);
            return;
        }

        var context = Current.CalculateText(TemplateParameters);
        if (conversationService.Current == null)
        {
            await conversationService.StartNewConversationAsync(_language, context);
            logger.TemplateInitiallyValid(context);
        }
        else
        {
            conversationService.Context = context;
            logger.TemplateContextChanged(context);
        }
    }

    public async Task UpdateTemplate(Template template)
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            await updateTemplateApi.UpdateTemplateAsync(userName, template);
            logger.TemplateUpdated(template);
        }
        catch (Exception e)
        {
            //TODO: Error Handling: Show error to user!
            logger.TemplateUpdateFailed(e, template);
        }

        await LoadTemplatesAsync();
    }

    public async Task DeleteTemplate(Template template)
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            await deleteTemplateApi.DeleteTemplateAsync(userName, template.Id);
            logger.TemplateDeleted(template);
        }
        catch (Exception e)
        {
            //TODO: Error Handling: Show error to user!
            logger.TemplateDeleteFailed(e, template);
        }

        await LoadTemplatesAsync();
    }

    public async Task<Template> GenerateEmptyTemplate(string name)
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var userName = authenticationState.User.Identity?.Name ?? "";
        return new Template
        {
            DefaultTemplate = false,
            Id = Guid.NewGuid(),
            Language = _language,
            Name = name,
            Text = "",
            UserName = userName
        };
    }
}
