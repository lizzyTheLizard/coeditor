using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace CoEditor.Rest;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(LoggingFilter))]
[SuppressMessage("ReSharper", "RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute",
    Justification = "Mie is not a good prefix, there might be other methods later")]
public class ProfileController(IGetProfileApi getProfileApi, IUpdateProfileApi updateProfileApi) : ControllerBase
{
    [Route("Mine/{language}")]
    [HttpGet]
    public async Task<ActionResult<Profile>> GetProfile(Language language)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await getProfileApi.GetProfileAsync(userName, language);
    }

    [Route("Mine/{language}")]
    [HttpPut]
    public async Task<ActionResult<Profile>> UpdateProfile(Language language, [FromBody] Profile profile)
    {
        var userName = User.Identity?.Name ?? throw new AuthenticationException("User not authenticated");
        return await updateProfileApi.UpdateProfileAsync(userName, profile);
    }
}
