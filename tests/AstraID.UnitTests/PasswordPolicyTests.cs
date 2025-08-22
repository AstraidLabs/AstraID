using Xunit;
using AstraID.Infrastructure.Services;
using FluentAssertions;

namespace AstraID.UnitTests;

public class PasswordPolicyTests
{
    [Theory]
    [InlineData("short")] // too short
    [InlineData("alllowercase123")] // missing uppercase
    [InlineData("ALLUPPERCASE123")] // missing lowercase
    [InlineData("NoDigitsHere!")] // missing digit
    public void WeakPasswords_ReturnError(string password)
    {
        var policy = new DefaultPasswordPolicy();
        var result = policy.ValidateStrength(password);
        result.Should().NotBeNull();
    }

    [Fact]
    public void StrongPassword_Passes()
    {
        var policy = new DefaultPasswordPolicy();
        var result = policy.ValidateStrength("StrongPass123");
        result.Should().BeNull();
    }
}
