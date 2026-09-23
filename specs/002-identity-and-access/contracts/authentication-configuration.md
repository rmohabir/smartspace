# Authentication Configuration Contract

## Modes

### Local development

```json
{
  "Authentication": {
    "Mode": "Development"
  },
  "DevelopmentIdentity": {
    "Enabled": true
  }
}
```

Expected behavior:

- Only valid in `ASPNETCORE_ENVIRONMENT=Development`.
- Uses `X-SmartSpace-Test-Subject` and `X-SmartSpace-Test-Role` for local demo and tests.
- Startup fails if this mode or `DevelopmentIdentity:Enabled=true` is used outside Development.

### Azure dev/prod Entra ID

```json
{
  "Authentication": {
    "Mode": "EntraId"
  },
  "EntraId": {
    "TenantId": "<from environment>",
    "ClientId": "<api app registration client id>",
    "Audience": "<api audience or api client id>"
  }
}
```

Expected behavior:

- Validates bearer tokens from the configured tenant.
- Uses `oid` or `sub` as the stable SmartSpace owner subject.
- Uses role/app role `Administrator` for administration.
- Startup fails when required Entra ID values are missing.

## Required Azure/Entra setup

- API app registration for SmartSpace API.
- Exposed API scope such as `access_as_user`.
- App role named `Administrator` assigned to authorized administrators.
- UI app registration for the Blazor WebAssembly client when interactive login is implemented.
- Redirect URI for local Entra testing: `http://localhost:3000/authentication/login-callback`.
- Redirect URI for Azure dev UI: the deployed UI callback URL.
- API configuration values supplied through environment/app settings, not source control.

## Claims contract

- Owner subject: `ClaimTypes.NameIdentifier`, mapped from token `oid` or `sub`.
- Administrator role: `Administrator` role claim.
- Employee: any authenticated user without `Administrator` role.
