using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(LoggingFilter))]
[Authorize]
public class InitializeConversationController(IInitializeConversationApi initializeConversationApi) : ControllerBase
{
    [HttpPost]
    [Route("Initialize")]
    public async Task<ActionResult<Conversation>> InitializeConversationAsync(
        [FromBody] InitializeConversationInput input)
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            throw new AuthenticationException();
        }

        return await initializeConversationApi.InitializeConversationAsync(userName, input);
    }
}
