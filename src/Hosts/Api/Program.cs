using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WineTracker.WineJournal;
using WineTracker.WineJournal.Application;
using WineTracker.WineJournal.Infrastructure;
using Journal = WineTracker.WineJournal.Application.WineJournal;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WineTracker")
    ?? throw new InvalidOperationException("Connection string 'WineTracker' is required.");

builder.Services.AddWineJournal(connectionString);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<WineJournalDbContext>();
    await database.Database.MigrateAsync();
}

var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new { status = "ok" }));

api.MapGet("/consumptions", async (Journal journal, CancellationToken cancellationToken) =>
    Results.Ok(await journal.ListHistoryAsync(cancellationToken)));

api.MapGet("/reorder-candidates", async (Journal journal, CancellationToken cancellationToken) =>
    Results.Ok(await journal.ListReorderCandidatesAsync(cancellationToken)));

api.MapPost("/consumptions", async (
    RecordConsumptionCommand command,
    Journal journal,
    CancellationToken cancellationToken) =>
{
    try
    {
        var result = await journal.RecordConsumptionAsync(command, cancellationToken);
        return Results.Created($"/api/consumptions/{result.Id}", result);
    }
    catch (ArgumentException exception)
    {
        var key = exception.ParamName ?? "request";
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [key] = [exception.Message]
        });
    }
});

api.MapPut("/consumptions/{consumptionId:guid}/reorder", async (
    Guid consumptionId,
    UpdateReorderIntentCommand command,
    Journal journal,
    CancellationToken cancellationToken) =>
{
    var updated = await journal.UpdateReorderIntentAsync(
        consumptionId,
        command.ReorderIntent,
        cancellationToken);
    return updated ? Results.NoContent() : Results.NotFound();
});

app.Run();
