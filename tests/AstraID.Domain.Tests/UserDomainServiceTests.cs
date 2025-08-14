namespace AstraID.Domain.Tests;

public class UserDomainServiceTests
{
    [Fact]
    public void Register_ShouldCreateUser_AndRaiseEvent_WhenEmailUnique() { }

    [Fact]
    public void ChangeEmail_ShouldFail_WhenUserInactive() { }
}
