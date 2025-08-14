using System;
namespace AstraID.Domain.Constants;

/// <summary>Security related constants.</summary>
public static class SecurityConstants
{
    /// <summary>Number of previous passwords remembered.</summary>
    public const int PasswordHistoryDepth = 5;

    /// <summary>Minimum interval between client secret rotations.</summary>
    public static readonly TimeSpan MinSecretRotationInterval = TimeSpan.FromDays(1);
}
