# Quickstart: SmartSpace identity and access

## Local development validation

1. Ensure local development config keeps development identity enabled:

   ```json
   "Authentication": { "Mode": "Development" },
   "DevelopmentIdentity": { "Enabled": true }
   ```

2. Run focused tests:

   ```powershell
   dotnet test tests/SmartSpace.IntegrationTests/SmartSpace.IntegrationTests.csproj --filter FullyQualifiedName~Security
   dotnet test tests/SmartSpace.UnitTests/SmartSpace.UnitTests.csproj --filter FullyQualifiedName~AuthorizationRulesTests
   ```

3. Start SmartSpace locally:

   ```powershell
   .\scripts\start-smartspace.ps1
   ```

Expected outcome: local demo requests using test identity headers continue to work only in Development.

## Entra ID readiness validation

1. Configure API for Entra ID mode without committing real values:

   ```powershell
   $env:Authentication__Mode = 'EntraId'
   $env:EntraId__TenantId = '<tenant id from environment>'
   $env:EntraId__ClientId = '<api app registration client id>'
   $env:EntraId__Audience = '<api audience or client id>'
   ```

2. Start the API.

Expected outcome: startup succeeds only when required values are present. Missing values fail fast with a configuration error.

## Azure setup checklist

- Create SmartSpace API app registration.
- Expose API scope `access_as_user`.
- Add app role `Administrator`.
- Assign administrators to the app role.
- Create SmartSpace UI app registration when UI login is implemented.
- Configure localhost and Azure dev redirect URIs for the UI.
- Store tenant/client/audience values in Azure app settings or local user secrets/environment variables.
