namespace SmartSpace.Api.Security;

public sealed class EntraIdOptions
{
    public const string SectionName = "EntraId";

    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? Audience { get; set; }

    public string EffectiveAudience => string.IsNullOrWhiteSpace(Audience) ? ClientId ?? string.Empty : Audience;

    public void Validate()
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(TenantId)) missing.Add($"{SectionName}:TenantId");
        if (string.IsNullOrWhiteSpace(ClientId)) missing.Add($"{SectionName}:ClientId");
        if (string.IsNullOrWhiteSpace(EffectiveAudience)) missing.Add($"{SectionName}:Audience");

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Entra ID authentication requires configuration values: {string.Join(", ", missing)}.");
        }
    }
}