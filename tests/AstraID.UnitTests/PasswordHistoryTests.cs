using Xunit;
using System;
using AstraID.Domain.Entities;
using FluentAssertions;

namespace AstraID.UnitTests;

public class PasswordHistoryTests
{
    [Fact]
    public void EnsureNotReused_Throws_WhenHashMatches()
    {
        var userId = Guid.NewGuid();
        var history = new[] { PasswordHistory.Record(userId, "hash1") };
        Action act = () => PasswordHistory.EnsureNotReused(history, "hash1");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EnsureNotReused_AllowsNewHash()
    {
        var userId = Guid.NewGuid();
        var history = new[] { PasswordHistory.Record(userId, "hash1") };
        Action act = () => PasswordHistory.EnsureNotReused(history, "hash2");
        act.Should().NotThrow();
    }
}
