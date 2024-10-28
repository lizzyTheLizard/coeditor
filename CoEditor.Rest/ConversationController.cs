using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(LoggingFilter))]
[Authorize]
public class ConversationController(
    IInitializeConversationApi initializeConversationApi,
    IHandleActionApi handleActionApi) : ControllerBase
{
    [HttpPost]
    [Route("Initialize")]
    public async Task<ActionResult<Conversation>> InitializeConversationAsync(
        [FromBody] InitializeConversationInput input)
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) throw new AuthenticationException();
        return await initializeConversationApi.InitializeConversationAsync(userName, input);
    }

    [HttpPost]
    [Route("Action")]
    public async Task<ActionResult<Conversation>> HandleActionAsync([FromBody] HandleNamedActionInput input)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        var conversation = await handleActionApi.HandleActionAsync(userName, input);
        return conversation;
    }

    [HttpPost]
    [Route("CustomAction")]
    public async Task<Conversation> HandleActionAsync([FromBody] HandleCustomActionInput input)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        var conversation = await handleActionApi.HandleActionAsync(userName, input);
        return conversation;
    }
}
