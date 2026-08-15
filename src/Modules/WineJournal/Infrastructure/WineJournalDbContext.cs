using Microsoft.EntityFrameworkCore;
using WineTracker.WineJournal.Domain;

namespace WineTracker.WineJournal.Infrastructure;

public sealed class WineJournalDbContext(DbContextOptions<WineJournalDbContext> options)
    : DbContext(options)
{
    public DbSet<Wine> Wines => Set<Wine>();

    public DbSet<WineConsumption> Consumptions => Set<WineConsumption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var wine = modelBuilder.Entity<Wine>();
        wine.ToTable("wines");
        wine.HasKey(item => item.Id);
        wine.Property(item => item.Producer).HasMaxLength(160).IsRequired();
        wine.Property(item => item.Name).HasMaxLength(160).IsRequired();
        wine.Property(item => item.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        wine.Property(item => item.Region).HasMaxLength(160);
        wine.Property(item => item.IdentityKey).HasMaxLength(400).IsRequired();
        wine.HasIndex(item => item.IdentityKey).IsUnique();

        var consumption = modelBuilder.Entity<WineConsumption>();
        consumption.ToTable("wine_consumptions");
        consumption.HasKey(item => item.Id);
        consumption.Property(item => item.Notes).HasMaxLength(2000);
        consumption.Property(item => item.ReorderIntent).HasConversion<string>().HasMaxLength(32).IsRequired();
        consumption.HasIndex(item => new { item.ConsumedOn, item.CreatedAt });
        consumption
            .HasOne(item => item.Wine)
            .WithMany(item => item.Consumptions)
            .HasForeignKey(item => item.WineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
