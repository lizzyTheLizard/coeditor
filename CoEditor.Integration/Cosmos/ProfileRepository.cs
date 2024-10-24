using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class ProfileRepository(CosmosDbContext dbContext) : IProfileRepository
{
    public async Task<Profile?> GetProfileAsync(string userName, Language language)
    {
        return await dbContext.Profiles
            .Where(t => t.Language == language)
            .Where(t => t.UserName == userName)
            .Select(t => new Profile { Text = t.Text, Language = t.Language })
            .FirstOrDefaultAsync();
    }
}
