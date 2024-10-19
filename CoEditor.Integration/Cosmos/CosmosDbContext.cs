using CoEditor.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class CosmosDbContext(DbContextOptions<CosmosDbContext> _options) : DbContext(_options)
{
    public DbSet<ProfileDocument> Profiles { get; set; }
    public DbSet<TemplateDocument> Templates { get; set; }
    public DbSet<ConversationDocument> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProfileDocument>()
            .ToContainer(nameof(Profile))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
        builder.Entity<TemplateDocument>()
            .ToContainer(nameof(Template))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
        builder.Entity<ConversationDocument>()
            .ToContainer(nameof(Conversation))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
    }
}
