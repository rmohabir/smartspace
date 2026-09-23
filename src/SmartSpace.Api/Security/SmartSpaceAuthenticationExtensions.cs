using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SmartSpace.Api.Security;

public static class SmartSpaceAuthenticationExtensions
{
    public static AuthenticationBuilder AddSmartSpaceAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        string mode,
        DevelopmentIdentityOptions developmentIdentity)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(developmentIdentity);

        return mode switch
        {
            AuthenticationModes.Development => AddDevelopmentAuthentication(services, environment, developmentIdentity),
            AuthenticationModes.EntraId => AddEntraIdAuthentication(services, configuration),
            _ => throw new InvalidOperationException(
                $"Unsupported authentication mode '{mode}'. Use '{AuthenticationModes.Development}' or '{AuthenticationModes.EntraId}'.")
        };
    }

    private static AuthenticationBuilder AddDevelopmentAuthentication(
        IServiceCollection services,
        IWebHostEnvironment environment,
        DevelopmentIdentityOptions developmentIdentity)
    {
        if (!developmentIdentity.Enabled)
        {
            throw new InvalidOperationException("Development authentication mode requires DevelopmentIdentity:Enabled=true.");
        }

        if (!developmentIdentity.IsAllowedInEnvironment(environment))
        {
            throw new InvalidOperationException("Development identity is only allowed in Development.");
        }

        return services
            .AddAuthentication(AuthenticationModes.Development)
            .AddScheme<AuthenticationSchemeOptions, DevelopmentIdentityHandler>(AuthenticationModes.Development, _ => { });
    }

    private static AuthenticationBuilder AddEntraIdAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var entraId = configuration.GetSection(EntraIdOptions.SectionName).Get<EntraIdOptions>()
            ?? new EntraIdOptions();
        entraId.Validate();

        return services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://login.microsoftonline.com/{entraId.TenantId}/v2.0";
                options.Audience = entraId.EffectiveAudience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        MapNameIdentifierClaim(context.Principal);
                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void MapNameIdentifierClaim(ClaimsPrincipal? principal)
    {
        if (principal?.Identity is not ClaimsIdentity identity ||
            principal.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
        {
            return;
        }

        var subject = principal.FindFirstValue("oid") ?? principal.FindFirstValue("sub");
        if (!string.IsNullOrWhiteSpace(subject))
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, subject));
        }
    }
}