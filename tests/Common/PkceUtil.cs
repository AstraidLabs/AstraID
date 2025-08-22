using System;
using System.Security.Cryptography;
using System.Text;

namespace AstraID.Tests.Common;

/// <summary>
/// Utilities for generating PKCE verifier and challenge pairs.
/// </summary>
public static class PkceUtil
{
    public static (string Verifier, string Challenge) CreatePair()
    {
        var verifierBytes = RandomNumberGenerator.GetBytes(32);
        var verifier = Base64UrlEncode(verifierBytes);
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
        var challenge = Base64UrlEncode(challengeBytes);
        return (verifier, challenge);
    }

    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
