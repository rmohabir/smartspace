# Implementation Plan: SmartSpace identity and access

**Feature**: `002-identity-and-access`
**Spec**: [spec.md](spec.md)
**Created**: 2026-09-23

## Summary

Add a minimal authentication and authorization foundation that keeps local DevelopmentIdentity available only in Development, introduces an explicit authentication mode, and prepares the API for Entra ID bearer token validation without hardcoded tenant values or secrets. The first implementation slice is API-side only; interactive Blazor/MSAL login is deferred until Entra app registrations exist.

## Technical Context

**Language/Version**: C# on .NET 10
**Primary Dependencies**: ASP.NET Core authentication/authorization, JWT bearer authentication for Entra ID mode
**Storage**: No new persistent entities
**Testing**: xUnit unit and integration tests in existing SmartSpace test projects
**Target Projects**: `src/SmartSpace.Api`, `tests/SmartSpace.UnitTests`, `tests/SmartSpace.IntegrationTests`
**Configuration**: `Authentication:Mode`, `DevelopmentIdentity:Enabled`, `EntraId:TenantId`, `EntraId:ClientId`, `EntraId:Audience`

## Constitution Check

- Traceable Business Behavior: PASS. Requirements map to user stories and task IDs.
- Backend-Enforced Authorization and Ownership: PASS. Owner identity remains server-side claims based.
- Development Identity Isolation: PASS. DevelopmentIdentity remains environment-gated and receives explicit mode checks.
- Secret and Personal Data Protection: PASS. No real tenant/client IDs or secrets are committed.
- Proposal Status Is Explicit: PASS. Full Blazor/MSAL login and Azure app registration values remain environment work and are not claimed as complete.

## Phase 0: Research Decisions

See [research.md](research.md).

## Phase 1: Design Artifacts

- [data-model.md](data-model.md): configuration and claim concepts; no database changes.
- [contracts/authentication-configuration.md](contracts/authentication-configuration.md): configuration contract and Azure/Entra setup expectations.
- [quickstart.md](quickstart.md): local and Entra-mode validation steps.

## Implementation Scope

Allowed files:

- `specs/002-identity-and-access/`
- `src/SmartSpace.Api/Program.cs`
- `src/SmartSpace.Api/Security/`
- `src/SmartSpace.Api/SmartSpace.Api.csproj`
- `src/SmartSpace.Api/appsettings*.json` and examples
- `tests/SmartSpace.UnitTests/*Authorization*`
- `tests/SmartSpace.IntegrationTests/Security/`
- `README.md` and `docs/decisions/` only when documenting required environment setup

Out of scope:

- User database, password management, role management UI
- Full Blazor WebAssembly MSAL login flow
- Hardcoded Entra tenant/client IDs or secrets
- Changes to existing reservation business rules

## Validation Strategy

1. Focused security tests:
   - `dotnet test tests/SmartSpace.IntegrationTests/SmartSpace.IntegrationTests.csproj --filter FullyQualifiedName~Security`
   - `dotnet test tests/SmartSpace.UnitTests/SmartSpace.UnitTests.csproj --filter FullyQualifiedName~AuthorizationRulesTests`
2. Broader compile gate:
   - `dotnet build SmartSpace.sln --no-restore`

## Post-Design Constitution Check

PASS. The plan keeps demo authentication isolated, keeps ownership server-side, and documents that production Entra configuration requires external Azure/Entra setup.
