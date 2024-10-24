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
    public TemplateParameter[] TemplateParameters { get; private set; } = [];

    public async Task SetLanguageAsync(Language language)
    {
        _language = language;
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var userName = authenticationState.User.Identity?.Name ?? "";
        Templates = await getTemplatesApi.GetTemplatesAsync(userName, language);
        logger.TemplatesLoaded(_language, Templates);
        await TemplateIdChangedAsync(Templates[0].Id);
    }

    public async Task TemplateIdChangedAsync(Guid templateId)
    {
        _templateId = templateId;
        var template = Templates.First(t => t.Id == templateId);
        TemplateParameters = template.GetTemplateParameters();
        conversationService.EndConversation();
        await ParameterChangedAsync();
    }

    public async Task ParameterChangedAsync()
    {
        var template = Templates.First(t => t.Id == _templateId);
        var valid = TemplateParameters.All(p => p.Valid);
        if (!valid)
        {
            logger.ParametersNotValid(TemplateParameters);
            return;
        }

        var context = template.CalculateText(TemplateParameters);
        if (conversationService.Current == null)
            await conversationService.StartNewConversationAsync(_language, context);
        else conversationService.Context = context;
    }
}
