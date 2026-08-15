namespace WineTracker.WineJournal.Domain;

public sealed class WineConsumption
{
    private WineConsumption()
    {
    }

    private WineConsumption(
        Guid id,
        Guid wineId,
        DateOnly consumedOn,
        int? rating,
        string? notes,
        ReorderIntent reorderIntent,
        DateTimeOffset createdAt)
    {
        Id = id;
        WineId = wineId;
        ConsumedOn = ValidateConsumedOn(consumedOn);
        Rating = ValidateRating(rating);
        Notes = ValidateNotes(notes);
        ReorderIntent = ValidateReorderIntent(reorderIntent);
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid WineId { get; private set; }

    public Wine Wine { get; private set; } = null!;

    public DateOnly ConsumedOn { get; private set; }

    public int? Rating { get; private set; }

    public string? Notes { get; private set; }

    public ReorderIntent ReorderIntent { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static WineConsumption Create(
        Guid wineId,
        DateOnly consumedOn,
        int? rating,
        string? notes,
        ReorderIntent reorderIntent,
        DateTimeOffset now)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException("A wine is required.", nameof(wineId));
        }

        return new WineConsumption(
            Guid.NewGuid(),
            wineId,
            consumedOn,
            rating,
            notes,
            reorderIntent,
            now);
    }

    public void SetReorderIntent(ReorderIntent reorderIntent) =>
        ReorderIntent = ValidateReorderIntent(reorderIntent);

    private static DateOnly ValidateConsumedOn(DateOnly consumedOn)
    {
        if (consumedOn == default)
        {
            throw new ArgumentException("A consumption date is required.", nameof(consumedOn));
        }

        return consumedOn;
    }

    private static ReorderIntent ValidateReorderIntent(ReorderIntent reorderIntent)
    {
        if (!Enum.IsDefined(reorderIntent))
        {
            throw new ArgumentOutOfRangeException(
                nameof(reorderIntent),
                "Reorder intent is not supported.");
        }

        return reorderIntent;
    }

    private static int? ValidateRating(int? rating)
    {
        if (rating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        }

        return rating;
    }

    private static string? ValidateNotes(string? notes)
    {
        var normalized = notes?.Trim();
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        if (normalized.Length > 2000)
        {
            throw new ArgumentException("Notes cannot exceed 2000 characters.", nameof(notes));
        }

        return normalized;
    }
}
