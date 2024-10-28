using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class ProfileRepository(CosmosDbContext dbContext) : IProfileRepository
{
    public async Task<Profile?> FindProfileAsync(string userName, Language language)
    {
        return await dbContext.Profiles
            .Where(t => t.Language == language)
            .Where(t => t.UserName == userName)
            .Select(t => new Profile { Text = t.Text, UserName = userName, Language = t.Language })
            .FirstOrDefaultAsync();
    }

    public async Task<Profile> CreateProfileAsync(string userName, Profile profile)
    {
        var document = new ProfileDocument
        {
            Id = Guid.NewGuid(), Language = profile.Language, UserName = userName, Text = profile.Text
        };
        dbContext.Add(document);
        await dbContext.SaveChangesAsync();
        return profile;
    }

    public async Task<Profile> UpdateProfileAsync(string userName, Profile profile)
    {
        var existingDocument = await dbContext.Profiles
            .Where(t => t.Language == profile.Language)
            .Where(t => t.UserName == userName)
            .FirstAsync();
        existingDocument.Text = profile.Text;
        await dbContext.SaveChangesAsync();
        return profile;
    }
}
