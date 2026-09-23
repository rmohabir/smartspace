# Data Model: SmartSpace identity and access

This feature introduces no new database tables or migrations.

## AuthenticationMode

Represents the configured API authentication boundary.

Fields:

- `Mode`: required effective value resolved from configuration. Supported values are `Development` and `EntraId`.

Rules:

- `Development` is allowed only when the host environment is Development.
- `EntraId` requires tenant and audience configuration before startup succeeds.
- Missing explicit mode may be resolved from existing local development settings for backward compatibility, but shared environments must not fall back to demo identity.

## EntraIdConfiguration

Represents non-secret Entra ID settings supplied by environment or app configuration.

Fields:

- `TenantId`: required in EntraId mode.
- `ClientId`: required in EntraId mode for the API app registration.
- `Audience`: optional override; defaults to API client ID when omitted.

Rules:

- Values must not be real checked-in tenant/client IDs in repository files.
- Secrets are not part of this feature because the API validates bearer tokens and does not require a client secret for inbound token validation.

## AuthenticatedSubject

Represents the current user after server-side authentication.

Fields:

- `SubjectId`: required stable identifier derived from `ClaimTypes.NameIdentifier`, mapped from Entra `oid` or `sub` when needed.
- `Roles`: optional collection of role claims.

Rules:

- Missing subject id means the user cannot create or manage reservations.
- Missing roles means the user is treated as Employee.
- `Administrator` role grants admin policy access; no other role grants administration.
