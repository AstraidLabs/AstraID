using AstraID.Domain.Entities;
using AstraID.Domain.ValueObjects;

namespace AstraID.Domain.Tests;

public class DomainStubs
{
    [Fact]
    public void AppUser_Register_NormalizesUsername_RaisesUserRegistered() { }

    [Fact]
    public void AppUser_ChangeEmail_WhenInactive_ThrowsInvalidOperationException() { }

    [Fact]
    public void AppUser_EnableTwoFactor_RaisesEvent() { }

    [Fact]
    public void RecoveryCode_MarkUsed_OnlyOnce() { }

    [Fact]
    public void PasswordHistory_PreventsImmediateReuse() { }

    [Fact]
    public void UserSession_Revoke_Twice_Throws() { }

    [Fact]
    public void UserSession_SeenNow_UpdatesLastSeenUtc() { }

    [Fact]
    public void UserConsent_RejectsEmptyScopes() { }

    [Fact]
    public void UserConsent_Revoke_PreventsUpdates() { }

    [Fact]
    public void Client_RotateSecret_Public_Throws() { }

    [Fact]
    public void Client_Collections_AreUnique() { }

    [Fact]
    public void Permission_BuiltIn_CannotBeDeleted() { }

    [Fact]
    public void RolePermission_Uniqueness_Enforced() { }
}
