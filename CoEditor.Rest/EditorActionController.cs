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
    public async Task<string> GetTemplates([FromBody] EditorActionInput editorActionInput)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _editorActionApi.HandleEditorActionAsync(userName, editorActionInput);
    }

}
