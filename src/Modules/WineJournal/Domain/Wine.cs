namespace WineTracker.WineJournal.Domain;

public sealed class Wine
{
    private Wine()
    {
    }

    private Wine(
        Guid id,
        string producer,
        string name,
        int? vintage,
        WineType type,
        string? region,
        DateTimeOffset createdAt)
    {
        Id = id;
        Producer = RequireText(producer, nameof(producer), 160);
        Name = RequireText(name, nameof(name), 160);
        Vintage = ValidateVintage(vintage);
        Type = type;
        Region = OptionalText(region, nameof(region), 160);
        IdentityKey = BuildIdentityKey(Producer, Name, Vintage, Type);
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Producer { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public int? Vintage { get; private set; }

    public WineType Type { get; private set; }

    public string? Region { get; private set; }

    public string IdentityKey { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public ICollection<WineConsumption> Consumptions { get; } = [];

    public static Wine Create(
        string producer,
        string name,
        int? vintage,
        WineType type,
        string? region,
        DateTimeOffset now) =>
        new(Guid.NewGuid(), producer, name, vintage, type, region, now);

    public static string BuildIdentityKey(
        string producer,
        string name,
        int? vintage,
        WineType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), "Wine type is not supported.");
        }

        var normalizedProducer = RequireText(producer, nameof(producer), 160).ToUpperInvariant();
        var normalizedName = RequireText(name, nameof(name), 160).ToUpperInvariant();
        var normalizedVintage = ValidateVintage(vintage)?.ToString() ?? "NV";
        return $"{normalizedProducer}|{normalizedName}|{normalizedVintage}|{type}";
    }

    private static string RequireText(string value, string parameterName, int maximumLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException("A value is required.", parameterName);
        }

        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException($"The value cannot exceed {maximumLength} characters.", parameterName);
        }

        return normalized;
    }

    private static string? OptionalText(string? value, string parameterName, int maximumLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException($"The value cannot exceed {maximumLength} characters.", parameterName);
        }

        return normalized;
    }

    private static int? ValidateVintage(int? vintage)
    {
        if (vintage is < 1800 or > 2200)
        {
            throw new ArgumentOutOfRangeException(nameof(vintage), "Vintage must be between 1800 and 2200.");
        }

        return vintage;
    }
}
