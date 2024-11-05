using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(LoggingFilter))]
public class UpdateProfileController(IUpdateProfileApi updateProfileApi) : ControllerBase
{
    [Route("Mine/{language}")]
    [HttpPut]
    public async Task<ActionResult<Profile>> UpdateProfile(Language language, [FromBody] Profile profile)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await updateProfileApi.UpdateProfileAsync(userName, profile);
    }
}
