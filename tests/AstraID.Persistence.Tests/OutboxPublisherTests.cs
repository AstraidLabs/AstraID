using System.Text.Json;
using AstraID.Domain.Abstractions;
using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Persistence.Messaging;
using AstraID.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AstraID.Persistence.Tests;

public class OutboxPublisherTests
{
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    }

    private sealed class TestDispatcher : IDomainEventDispatcher
    {
        public List<IDomainEvent> Events { get; } = new();
        public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
        {
            Events.AddRange(events);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task PublishPendingAsync_DispatchesEventsAndMarksProcessed()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new TestDbContext(options);

        var evt = new ClientRegistered(Guid.NewGuid(), "client1");
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(ClientRegistered).AssemblyQualifiedName!,
            PayloadJson = JsonSerializer.Serialize(evt, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            CreatedUtc = DateTime.UtcNow,
            Attempts = 0
        };
        db.OutboxMessages.Add(message);
        await db.SaveChangesAsync();

        var dispatcher = new TestDispatcher();
        var publisher = new OutboxPublisher(db, dispatcher, NullLogger<OutboxPublisher>.Instance);

        await publisher.PublishPendingAsync();
        await db.Entry(message).ReloadAsync();

        Assert.NotNull(message.ProcessedUtc);
        Assert.Single(dispatcher.Events);
    }
}
