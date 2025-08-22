using Xunit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace AstraID.SecurityTests;

public class TokenTamperingTests
{
    [Fact]
    public void ModifiedToken_IsRejected()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey_supersecretkey123456"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateEncodedJwt("issuer", "aud", new ClaimsIdentity(new[] { new Claim("sub", "1") }), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow, creds);

        // tamper payload without resigning
        var parts = token.Split('.');
        var payload = parts[1];
        var bytes = Base64UrlEncoder.DecodeBytes(payload);
        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(bytes)!;
        dict["sub"] = "2";
        var tamperedPayload = Base64UrlEncoder.Encode(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(dict));
        var tampered = $"{parts[0]}.{tamperedPayload}.{parts[2]}";

        Action act = () => handler.ValidateToken(tampered, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = key,
            ValidateLifetime = false
        }, out _);

        act.Should().Throw<SecurityTokenInvalidSignatureException>();
    }
}
