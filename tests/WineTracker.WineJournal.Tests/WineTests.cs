using WineTracker.WineJournal.Domain;

namespace WineTracker.WineJournal.Tests;

public sealed class WineTests
{
    [Fact]
    public void Identity_key_normalizes_case_and_whitespace()
    {
        var first = Wine.BuildIdentityKey("  Bodega Norte ", "Reserva", 2020, WineType.Red);
        var second = Wine.BuildIdentityKey("bodega norte", " reserva ", 2020, WineType.Red);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Create_rejects_an_invalid_vintage()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Wine.Create("Producer", "Label", 1700, WineType.Red, null, DateTimeOffset.UtcNow));

        Assert.Equal("vintage", exception.ParamName);
    }
}
