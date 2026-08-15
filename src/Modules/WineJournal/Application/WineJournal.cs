using Microsoft.EntityFrameworkCore;
using WineTracker.WineJournal.Domain;
using WineTracker.WineJournal.Infrastructure;

namespace WineTracker.WineJournal.Application;

public sealed class WineJournal(WineJournalDbContext database, TimeProvider timeProvider)
{
    public async Task<ConsumptionResult> RecordConsumptionAsync(
        RecordConsumptionCommand command,
        CancellationToken cancellationToken = default)
    {
        var identityKey = Wine.BuildIdentityKey(
            command.Producer,
            command.Name,
            command.Vintage,
            command.Type);

        var wine = await database.Wines
            .SingleOrDefaultAsync(candidate => candidate.IdentityKey == identityKey, cancellationToken);

        if (wine is null)
        {
            wine = Wine.Create(
                command.Producer,
                command.Name,
                command.Vintage,
                command.Type,
                command.Region,
                timeProvider.GetUtcNow());
            database.Wines.Add(wine);
        }

        var consumption = WineConsumption.Create(
            wine.Id,
            command.ConsumedOn,
            command.Rating,
            command.Notes,
            command.ReorderIntent,
            timeProvider.GetUtcNow());

        database.Consumptions.Add(consumption);
        await database.SaveChangesAsync(cancellationToken);

        return new ConsumptionResult(consumption.Id, wine.Id);
    }

    public Task<List<WineHistoryItem>> ListHistoryAsync(CancellationToken cancellationToken = default) =>
        database.Consumptions
            .AsNoTracking()
            .OrderByDescending(consumption => consumption.ConsumedOn)
            .ThenByDescending(consumption => consumption.CreatedAt)
            .Select(consumption => new WineHistoryItem(
                consumption.Id,
                consumption.WineId,
                consumption.Wine.Producer,
                consumption.Wine.Name,
                consumption.Wine.Vintage,
                consumption.Wine.Type,
                consumption.Wine.Region,
                consumption.ConsumedOn,
                consumption.Rating,
                consumption.Notes,
                consumption.ReorderIntent))
            .ToListAsync(cancellationToken);

    public async Task<List<ReorderCandidate>> ListReorderCandidatesAsync(
        CancellationToken cancellationToken = default)
    {
        var consumptions = await database.Consumptions
            .AsNoTracking()
            .Include(consumption => consumption.Wine)
            .OrderByDescending(consumption => consumption.ConsumedOn)
            .ThenByDescending(consumption => consumption.CreatedAt)
            .ToListAsync(cancellationToken);

        return consumptions
            .GroupBy(consumption => consumption.WineId)
            .Select(group => new { Latest = group.First(), Count = group.Count() })
            .Where(item => item.Latest.ReorderIntent == ReorderIntent.Yes)
            .Select(item => new ReorderCandidate(
                item.Latest.WineId,
                item.Latest.Wine.Producer,
                item.Latest.Wine.Name,
                item.Latest.Wine.Vintage,
                item.Latest.Wine.Type,
                item.Latest.Wine.Region,
                item.Latest.ConsumedOn,
                item.Latest.Rating,
                item.Count))
            .OrderByDescending(candidate => candidate.LastConsumedOn)
            .ThenBy(candidate => candidate.Producer)
            .ThenBy(candidate => candidate.Name)
            .ToList();
    }

    public async Task<bool> UpdateReorderIntentAsync(
        Guid consumptionId,
        ReorderIntent reorderIntent,
        CancellationToken cancellationToken = default)
    {
        var consumption = await database.Consumptions.FindAsync([consumptionId], cancellationToken);
        if (consumption is null)
        {
            return false;
        }

        consumption.SetReorderIntent(reorderIntent);
        await database.SaveChangesAsync(cancellationToken);
        return true;
    }
}
