using AstraID.Domain.Events;
using AstraID.Domain.Primitives;
using AstraID.Infrastructure.Persistence.Interceptors;
using AstraID.Persistence.Messaging;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AstraID.Persistence.Tests;

public class DomainEventsCollectorInterceptorTests
{
    private sealed class TestAggregate : AggregateRoot<Guid>
    {
        public new void Raise(IDomainEvent evt) => base.Raise(evt);
    }

    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TestAggregate> Aggregates => Set<TestAggregate>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    }

    [Fact]
    public async Task SavingChanges_WritesOutboxMessagesAndClearsDomainEvents()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new DomainEventsCollectorInterceptor())
            .Options;

        var aggregate = new TestAggregate();
        aggregate.Raise(new ClientRegistered(Guid.NewGuid(), "client1"));

        await using var db = new TestDbContext(options);
        await db.Aggregates.AddAsync(aggregate);
        await db.SaveChangesAsync();

        Assert.Empty(aggregate.DomainEvents);
        Assert.Equal(1, await db.OutboxMessages.CountAsync());
    }
}
