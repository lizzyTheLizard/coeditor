using CoEditor.Domain.Incomming;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EditorActionController(IEditorActionApi _editorActionApi) : ControllerBase
{
    [HttpPost]
    [Route("Initialize")]
    public async Task<string> InitializeConversationAsync([FromBody] InitializeConversationInput input)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.InitializeConversationAsync(userName, input);
    }

    [HttpPost]
    [Route("Action")]
    public async Task<string> HandleEditorCommandAsync([FromBody] HandleEditorCommandInput input)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.HandleEditorCommandAsync(userName, input);
    }

    [HttpPost]
    [Route("CustomAction")]
    public async Task<string> HandleCustomEditorCommandAsync([FromBody] HandleCustomEditorCommandInput input)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.HandleCustomEditorCommandAsync(userName, input);
    }
}
