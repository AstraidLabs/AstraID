using System;
using AstraID.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AstraID.Persistence.Internal;

internal static class ValueObjectConverters
{
    public static readonly ValueConverter<Email, string> EmailConverter =
        new(e => e.Value, v => Email.Create(v));

    public static readonly ValueComparer<Email> EmailComparer = new(
        (l, r) => string.Equals(l!.Value, r!.Value, StringComparison.OrdinalIgnoreCase),
        v => v!.Value.GetHashCode(StringComparison.OrdinalIgnoreCase),
        v => Email.Create(v!.Value));

    public static readonly ValueConverter<Scope, string> ScopeConverter =
        new(s => s.Value, v => Scope.Create(v));

    public static readonly ValueComparer<Scope> ScopeComparer = new(
        (l, r) => string.Equals(l!.Value, r!.Value, StringComparison.Ordinal),
        v => v!.Value.GetHashCode(StringComparison.Ordinal),
        v => Scope.Create(v!.Value));

    public static readonly ValueConverter<RedirectUri, string> RedirectUriConverter =
        new(r => r.Value, v => RedirectUri.Create(v));

    public static readonly ValueComparer<RedirectUri> RedirectUriComparer = new(
        (l, r) => string.Equals(l!.Value, r!.Value, StringComparison.OrdinalIgnoreCase),
        v => v!.Value.GetHashCode(StringComparison.OrdinalIgnoreCase),
        v => RedirectUri.Create(v!.Value));

    public static readonly ValueConverter<HashedSecret, string> HashedSecretConverter =
        new(h => h.Value, v => HashedSecret.FromHash(v));

    public static readonly ValueComparer<HashedSecret> HashedSecretComparer = new(
        (l, r) => string.Equals(l!.Value, r!.Value, StringComparison.Ordinal),
        v => v!.Value.GetHashCode(StringComparison.Ordinal),
        v => HashedSecret.FromHash(v!.Value));

    public static readonly ValueConverter<IpAddress, string> IpAddressConverter =
        new(i => i.Value, v => IpAddress.Create(v));

    public static readonly ValueComparer<IpAddress> IpAddressComparer = new(
        (l, r) => string.Equals(l!.Value, r!.Value, StringComparison.Ordinal),
        v => v!.Value.GetHashCode(StringComparison.Ordinal),
        v => IpAddress.Create(v!.Value));

    public static readonly ValueConverter<UserAgent, string> UserAgentConverter =
        new(a => a.Value, v => UserAgent.Create(v));

    public static readonly ValueComparer<UserAgent> UserAgentComparer = new(
        (l, r) => string.Equals(l!.Value, r!.Value, StringComparison.Ordinal),
        v => v!.Value.GetHashCode(StringComparison.Ordinal),
        v => UserAgent.Create(v!.Value));
}
