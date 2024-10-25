using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class TemplateService(
    ConversationService conversationService,
    IGetTemplatesApi getTemplatesApi,
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
}
