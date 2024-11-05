using System.Security.Authentication;
using CoEditor.Domain.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(LoggingFilter))]
public class DeleteTemplateController(IDeleteTemplateApi deleteTemplateApi) : ControllerBase
{
    [Route("{id}")]
    [HttpDelete]
    public async Task<ActionResult> DeleteTemplate(string id)
    {
        var guid = Guid.Parse(id);
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        await deleteTemplateApi.DeleteTemplateAsync(userName, guid);
        return Ok();
    }
}
