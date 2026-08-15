using WineTracker.WineJournal.Domain;

namespace WineTracker.WineJournal.Application;

public sealed record RecordConsumptionCommand(
    string Producer,
    string Name,
    int? Vintage,
    WineType Type,
    string? Region,
    DateOnly ConsumedOn,
    int? Rating,
    string? Notes,
    ReorderIntent ReorderIntent);

public sealed record UpdateReorderIntentCommand(ReorderIntent ReorderIntent);

public sealed record ConsumptionResult(Guid Id, Guid WineId);

public sealed record WineHistoryItem(
    Guid ConsumptionId,
    Guid WineId,
    string Producer,
    string Name,
    int? Vintage,
    WineType Type,
    string? Region,
    DateOnly ConsumedOn,
    int? Rating,
    string? Notes,
    ReorderIntent ReorderIntent);

public sealed record ReorderCandidate(
    Guid WineId,
    string Producer,
    string Name,
    int? Vintage,
    WineType Type,
    string? Region,
    DateOnly LastConsumedOn,
    int? LastRating,
    int TimesConsumed);
