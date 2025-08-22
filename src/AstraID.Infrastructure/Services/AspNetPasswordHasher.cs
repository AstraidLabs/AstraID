using AstraID.Domain.Abstractions;
using AstraID.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AstraID.Infrastructure.Services;

/// <summary>Adapter over ASP.NET Identity password hasher for domain usage.</summary>
public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly IPasswordHasher<AppUser> _hasher;

    public AspNetPasswordHasher(IPasswordHasher<AppUser> hasher)
        => _hasher = hasher;

    /// <inheritdoc />
    public string Hash(string password)
        => _hasher.HashPassword(null!, password);

    /// <inheritdoc />
    public bool Verify(string hash, string password)
        => _hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
