using AstraID.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AstraID.Infrastructure.Messaging.Background;

/// <summary>
/// Periodically publishes pending outbox messages.
/// </summary>
public sealed class OutboxHostedService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly TimeSpan _interval;

    public OutboxHostedService(IServiceProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        var seconds = int.TryParse(configuration["ASTRAID_OUTBOX__POLL_INTERVAL_SECONDS"], out var s) ? s : 5;
        _interval = TimeSpan.FromSeconds(seconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IOutboxPublisher>();
            await publisher.PublishPendingAsync(stoppingToken).ConfigureAwait(false);
            await Task.Delay(_interval, stoppingToken).ConfigureAwait(false);
        }
    }
}
