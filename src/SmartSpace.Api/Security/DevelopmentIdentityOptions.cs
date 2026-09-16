using Microsoft.AspNetCore.Hosting;

namespace SmartSpace.Api.Security;

public sealed class DevelopmentIdentityOptions
{
    public const string SectionName = "DevelopmentIdentity";
    public bool Enabled { get; set; }

    public bool IsAllowedInEnvironment(IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);
        return Enabled && environment.IsDevelopment();
    }
}
