using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SmartSpace.Api.Security;

namespace SmartSpace.IntegrationTests.Security;

public sealed class DevelopmentIdentityTests
{
    [Fact]
    public void Development_identity_is_allowed_only_in_development()
    {
        var options = new DevelopmentIdentityOptions { Enabled = true };
        var developmentEnvironment = new FakeWebHostEnvironment { EnvironmentName = Environments.Development };
        var productionEnvironment = new FakeWebHostEnvironment { EnvironmentName = Environments.Production };

        Assert.True(options.IsAllowedInEnvironment(developmentEnvironment));
        Assert.False(options.IsAllowedInEnvironment(productionEnvironment));
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "SmartSpace.Api";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
