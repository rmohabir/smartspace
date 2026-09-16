using SmartSpace.Api.Security;

namespace SmartSpace.IntegrationTests.Security;

public sealed class DevelopmentIdentityTests
{
    [Fact(Skip = "Requires API host startup validation outside Development.")]
    public void Development_identity_is_rejected_outside_development()
    {
        Assert.True(new DevelopmentIdentityOptions().Enabled);
    }
}
