using Microsoft.EntityFrameworkCore;

namespace CoEditor.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Template> Templates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Profile>()
            .ToContainer(nameof(Profile))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
        builder.Entity<Template>()
            .ToContainer(nameof(Template))
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
    }
}
