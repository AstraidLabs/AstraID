using AstraID.Domain.Entities;
using AstraID.Infrastructure.OpenIddict;
using Moq;
using OpenIddict.Abstractions;
using Xunit;

namespace AstraID.Persistence.Tests;

public class ClientApplicationBridgeTests
{
    [Fact]
    public async Task EnsureCreatedAsync_CreatesApplicationWhenMissing()
    {
        var manager = new Mock<IOpenIddictApplicationManager>();
        manager.Setup(m => m.FindByClientIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)null);

        OpenIddictApplicationDescriptor? captured = null;
        manager.Setup(m => m.CreateAsync(It.IsAny<OpenIddictApplicationDescriptor>(), It.IsAny<CancellationToken>()))
            .Callback<OpenIddictApplicationDescriptor, CancellationToken>((d, _) => captured = d)
            .ReturnsAsync(new object());

        var bridge = new ClientApplicationBridge(manager.Object);
        var client = Client.Register("client1", "Client One", ClientType.Public);

        await bridge.EnsureCreatedAsync(client);

        Assert.NotNull(captured);
        Assert.Equal("client1", captured!.ClientId);
        manager.Verify(m => m.CreateAsync(It.IsAny<OpenIddictApplicationDescriptor>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyChangesAsync_UpdatesExistingApplication()
    {
        var manager = new Mock<IOpenIddictApplicationManager>();
        var app = new object();
        manager.Setup(m => m.FindByClientIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(app);
        manager.Setup(m => m.PopulateAsync(It.IsAny<OpenIddictApplicationDescriptor>(), app, It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        OpenIddictApplicationDescriptor? updated = null;
        manager.Setup(m => m.UpdateAsync(app, It.IsAny<OpenIddictApplicationDescriptor>(), It.IsAny<CancellationToken>()))
            .Callback<object, OpenIddictApplicationDescriptor, CancellationToken>((_, d, _) => updated = d)
            .Returns(ValueTask.CompletedTask);

        var bridge = new ClientApplicationBridge(manager.Object);
        var client = Client.Register("client1", "Client One", ClientType.Public);

        await bridge.ApplyChangesAsync(client);

        Assert.NotNull(updated);
        Assert.Equal("client1", updated!.ClientId);
        manager.Verify(m => m.UpdateAsync(app, It.IsAny<OpenIddictApplicationDescriptor>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
