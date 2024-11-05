﻿using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(LoggingFilter))]
public class UpdateTemplateController(IUpdateTemplateApi updateTemplateApi) : ControllerBase
{
    [Route("{id}")]
    [HttpPut]
    public async Task<ActionResult<Template>> UpdateTemplate(string id, [FromBody] Template template)
    {
        var guid = Guid.Parse(id);
        if (guid != template.Id)
        {
            throw new ArgumentException("ID in path and body must be the same");
        }

        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await updateTemplateApi.UpdateTemplateAsync(userName, template);
    }
}
