using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WineTracker.WineJournal.Application;
using WineTracker.WineJournal.Infrastructure;

namespace WineTracker.WineJournal;

public static class DependencyInjection
{
    public static IServiceCollection AddWineJournal(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<WineJournalDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<Application.WineJournal>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
