using System.Security.Claims;
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

    [Fact]
    public void Authentication_mode_defaults_to_development_when_development_identity_is_enabled()
    {
        var mode = new SmartSpaceAuthenticationOptions()
            .ResolveMode(new DevelopmentIdentityOptions { Enabled = true });

        Assert.Equal(AuthenticationModes.Development, mode);
    }

    [Fact]
    public void Authentication_mode_uses_explicit_entra_id_mode()
    {
        var mode = new SmartSpaceAuthenticationOptions { Mode = " EntraId " }
            .ResolveMode(new DevelopmentIdentityOptions { Enabled = true });

        Assert.Equal(AuthenticationModes.EntraId, mode);
    }

    [Fact]
    public void Entra_id_options_require_tenant_and_client_configuration()
    {
        var options = new EntraIdOptions();

        var exception = Assert.Throws<InvalidOperationException>(options.Validate);

        Assert.Contains("EntraId:TenantId", exception.Message);
        Assert.Contains("EntraId:ClientId", exception.Message);
    }

    [Fact]
    public void Entra_id_audience_defaults_to_client_id()
    {
        var options = new EntraIdOptions
        {
            TenantId = "tenant-id",
            ClientId = "api-client-id"
        };

        options.Validate();

        Assert.Equal("api-client-id", options.EffectiveAudience);
    }

    [Fact]
    public void Entra_oid_is_mapped_to_server_side_owner_subject_claim()
    {
        var identity = new ClaimsIdentity([new Claim("oid", "entra-object-id")], "Bearer");
        var principal = new ClaimsPrincipal(identity);

        SmartSpaceAuthenticationExtensions.MapNameIdentifierClaim(principal);

        Assert.Equal("entra-object-id", principal.FindFirstValue(ClaimTypes.NameIdentifier));
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
