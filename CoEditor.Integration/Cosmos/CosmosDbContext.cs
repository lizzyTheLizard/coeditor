using CoEditor.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class CosmosDbContext(DbContextOptions<CosmosDbContext> options) : DbContext(options)
{
    public DbSet<ProfileDocument> Profiles { get; init; }

    public DbSet<TemplateDocument> Templates { get; init; }

    public DbSet<ConversationDocument> Conversations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProfileDocument>()
            .ToContainer(nameof(Profile))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
        modelBuilder.Entity<TemplateDocument>()
            .ToContainer(nameof(Template))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
        modelBuilder.Entity<ConversationDocument>()
            .ToContainer(nameof(Conversation))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
    }
}
