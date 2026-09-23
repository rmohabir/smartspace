# Tasks: SmartSpace identity and access

**Input**: [spec.md](spec.md), [plan.md](plan.md), [data-model.md](data-model.md), [research.md](research.md), [contracts/authentication-configuration.md](contracts/authentication-configuration.md), [quickstart.md](quickstart.md)

## Dependencies and delivery order

```text
T001-T003 Setup
  -> T004-T007 API authentication foundation
    -> T008-T010 Documentation and validation
```

## Phase 1: Setup

- [X] T001 Create new Speckit feature artifacts in `specs/002-identity-and-access/`; Done when spec, checklist, plan, research, data model, contract, quickstart, and tasks exist.
- [X] T002 Define the authentication configuration contract in `specs/002-identity-and-access/contracts/authentication-configuration.md`; Done when local Development and EntraId modes plus Azure/Entra prerequisites are documented.
- [X] T003 Establish implementation scope in `specs/002-identity-and-access/plan.md`; Done when allowed files and out-of-scope identity features are explicit.

## Phase 2: API authentication foundation

- [X] T004 Add authentication mode and Entra ID configuration options in `src/SmartSpace.Api/Security/`; Depends: T001-T003; Done when `Development` and `EntraId` modes are represented, Entra required values validate, and no tenant/client values are hardcoded (FR-001, FR-002, FR-006, FR-007).
- [X] T005 Update authentication registration in `src/SmartSpace.Api/Program.cs` and `src/SmartSpace.Api/SmartSpace.Api.csproj`; Depends: T004; Done when Development mode uses `DevelopmentIdentityHandler`, EntraId mode uses JWT bearer validation, `oid` or `sub` maps to `ClaimTypes.NameIdentifier`, and roles map to `Administrator` authorization (FR-003-FR-008).
- [X] T006 Update safe configuration examples in `src/SmartSpace.Api/appsettings.json` and `src/SmartSpace.Api/appsettings.Development.example.json`; Depends: T004-T005; Done when examples show mode names and placeholders only (FR-006, FR-007, FR-009).
- [X] T007 Add focused security tests in `tests/SmartSpace.IntegrationTests/Security/DevelopmentIdentityTests.cs` and `tests/SmartSpace.UnitTests/AuthorizationRulesTests.cs`; Depends: T004-T006; Done when tests cover development-only guard, Entra missing configuration validation, owner subject mapping contract, and Administrator role behavior (FR-001-FR-008).

## Phase 3: Documentation and validation

- [X] T008 Update `README.md` with the local development auth mode and Azure/Entra dev setup checklist; Depends: T004-T007; Done when developers know how to run locally without Entra and what Azure must provide later (FR-009).
- [X] T009 Run focused validation commands from `specs/002-identity-and-access/quickstart.md`; Depends: T004-T008; Done when focused security/unit tests pass and output is recorded in the loop result (SC-001-SC-004).
- [X] T010 Run `dotnet build SmartSpace.sln --no-restore`; Depends: T009; Done when the solution compiles after the auth foundation changes.

## Traceability matrix

| Requirement | Primary tasks | Evidence |
|---|---|---|
| FR-001, FR-002 | T004, T005, T007 | Security tests |
| FR-003, FR-004, FR-005, FR-008 | T005, T007 | Authorization and claims tests |
| FR-006, FR-007 | T004-T006 | Config validation tests and examples |
| FR-009 | T002, T008 | README and contract docs |

## MVP and implementation strategy

MVP is Phase 2 plus focused validation. Full Blazor/MSAL login is intentionally deferred until the Entra app registrations and redirect URIs exist.
