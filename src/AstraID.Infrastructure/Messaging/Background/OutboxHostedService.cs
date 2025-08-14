using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AstraID.Infrastructure.Messaging.Background;

/// <summary>
/// Background service that publishes messages from the outbox.
/// </summary>
public class OutboxHostedService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public OutboxHostedService(IServiceProvider provider) => _provider = provider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AstraIdDbContext>();

            var messages = await db.OutboxMessages
                .Where(m => m.ProcessedUtc == null)
                .OrderBy(m => m.CreatedUtc)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                // For now, simply mark as processed
                message.ProcessedUtc = DateTime.UtcNow;
                // No deserialization to domain event types here, kept simple
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }
}
