using WineTracker.WineJournal.Domain;

namespace WineTracker.WineJournal.Tests;

public sealed class WineConsumptionTests
{
    [Fact]
    public void Create_preserves_explicit_reorder_intent()
    {
        var consumption = WineConsumption.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 15),
            5,
            "Bright and balanced",
            ReorderIntent.Yes,
            DateTimeOffset.UtcNow);

        Assert.Equal(ReorderIntent.Yes, consumption.ReorderIntent);
    }

    [Fact]
    public void Create_rejects_rating_outside_the_supported_scale()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => WineConsumption.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 15),
            6,
            null,
            ReorderIntent.Undecided,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Create_requires_a_consumption_date()
    {
        Assert.Throws<ArgumentException>(() => WineConsumption.Create(
            Guid.NewGuid(),
            default,
            null,
            null,
            ReorderIntent.Undecided,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Setting_intent_does_not_change_the_rating()
    {
        var consumption = WineConsumption.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 15),
            3,
            null,
            ReorderIntent.Undecided,
            DateTimeOffset.UtcNow);

        consumption.SetReorderIntent(ReorderIntent.No);

        Assert.Equal(3, consumption.Rating);
        Assert.Equal(ReorderIntent.No, consumption.ReorderIntent);
    }
}
