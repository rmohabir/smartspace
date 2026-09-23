# Research: SmartSpace identity and access

## Authentication mode

**Decision**: Add an explicit authentication mode with `Development` and `EntraId` values. Keep backward compatibility by allowing existing `DevelopmentIdentity:Enabled=true` to resolve to Development mode in local Development.

**Rationale**: The current single flag protects local demo auth outside Development, but it does not express the intended shared-environment auth boundary. A mode makes startup behavior reviewable and testable without adding a user database.

**Alternatives considered**:

- Use only `DevelopmentIdentity:Enabled`: too implicit for Azure dev/prod readiness.
- Always require Entra ID locally: slows development and automated tests.
- Build a custom username/password store: out of scope and less secure than Entra.

## Entra ID boundary

**Decision**: API supports Entra ID bearer token validation through configuration-supplied tenant and audience values. Real tenant/client IDs are never committed.

**Rationale**: The API is the authorization boundary. Validating bearer tokens server-side fits the existing Minimal API model and keeps ownership derived from claims.

**Alternatives considered**:

- Trust UI-provided identity headers in Azure: violates backend-enforced authorization.
- Implement full UI MSAL login in this slice: requires app registration details and redirect URI decisions not yet available.

## Ownership claim mapping

**Decision**: Map Entra `oid` or `sub` to `ClaimTypes.NameIdentifier`, which the existing reservation endpoints already use.

**Rationale**: This preserves the current reservation and ownership code path while making the token source production-like.

**Alternatives considered**:

- Rewrite all reservation code to read `oid` directly: broader change with no functional benefit.
- Store email as owner: emails can change and are less stable than object/subject identifiers.

## Administrator authorization

**Decision**: Use role/app role `Administrator` and configure role claims so `User.IsInRole("Administrator")` continues to be the single API check.

**Rationale**: One app role is enough for release 1. It avoids group overage and role-management UI complexity.

**Alternatives considered**:

- Entra group authorization: more tenant-specific and harder to test locally.
- SmartSpace-managed roles: adds persistence and admin UI outside the requested scope.
