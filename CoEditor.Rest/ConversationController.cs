using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ConversationController(IConversationApi _editorActionApi) : ControllerBase
{
    [HttpPost]
    [Route("Initialize")]
    public async Task<Conversation> InitializeConversationAsync([FromBody] HandleInitialActionInput input)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.InitializeConversationAsync(userName, input);
    }

    [HttpPost]
    [Route("Action")]
    public async Task<Conversation> HandleActionAsync([FromBody] HandleNamedActionInput input)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.HandleActionAsync(userName, input);
    }

    [HttpPost]
    [Route("CustomAction")]
    public async Task<Conversation> HandleActionAsync([FromBody] HandleCustomActionInput input)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.HandleActionAsync(userName, input);
    }
}
