using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TemplateController(ITemplateApi _templateApi) : ControllerBase
{
    [Route("Mine/{language}")]
    [HttpGet]
    public async Task<IEnumerable<Template>> GetTemplates(Language language)
    {
        var userName = User?.Identity?.Name ?? throw new Exception("User not authenticated");
        return await _templateApi.GetTemplatesAsync(language, userName);
    }
}
