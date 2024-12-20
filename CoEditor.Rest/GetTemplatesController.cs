﻿using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/Template")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(LoggingFilter))]
public class GetTemplatesController(IGetTemplatesApi getTemplatesApi) : ControllerBase
{
    [Route("Mine/{language}")]
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
    public async Task<ActionResult<IEnumerable<Template>>> GetTemplates(Language language)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await getTemplatesApi.GetTemplatesAsync(userName, language);
    }
}
