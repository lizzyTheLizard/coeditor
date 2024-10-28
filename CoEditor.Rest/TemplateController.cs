using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(LoggingFilter))]
public class TemplateController(
    IGetTemplatesApi getTemplatesApi,
    IUpdateTemplateApi updateTemplateApi,
    IDeleteTemplateApi deleteTemplateApi) : ControllerBase
{
    [Route("Mine/{language}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Template>>> GetTemplates(Language language)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await getTemplatesApi.GetTemplatesAsync(userName, language);
    }

    [Route("{id}")]
    [HttpPut]
    public async Task<ActionResult<Template>> UpdateTemplate(string id, [FromBody] Template template)
    {
        var guid = Guid.Parse(id);
        if (guid != template.Id) throw new ArgumentException("ID in path and body must be the same");
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await updateTemplateApi.UpdateTemplateAsync(userName, template);
    }

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
