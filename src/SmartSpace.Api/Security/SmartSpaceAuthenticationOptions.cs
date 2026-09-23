namespace SmartSpace.Api.Security;

public sealed class SmartSpaceAuthenticationOptions
{
    public const string SectionName = "Authentication";
    public string? Mode { get; set; }

    public string ResolveMode(DevelopmentIdentityOptions developmentIdentity)
    {
        ArgumentNullException.ThrowIfNull(developmentIdentity);

        if (!string.IsNullOrWhiteSpace(Mode))
        {
            return Mode.Trim();
        }

        return developmentIdentity.Enabled ? AuthenticationModes.Development : AuthenticationModes.EntraId;
    }
}