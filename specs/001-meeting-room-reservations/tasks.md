# Tasks: SmartSpace release 1 vergaderruimtes

**Input**: [spec.md](spec.md), [plan.md](plan.md), [data-model.md](data-model.md),
[contracts/openapi.yaml](contracts/openapi.yaml), [quickstart.md](quickstart.md)

**Status**: Planning only. These tasks describe future implementation work; no task
has been executed by this command.

## Traceability convention

- `FR-xxx` refers to a functional requirement in `spec.md`.
- `AC-USx.y` refers to acceptance scenario `y` under user story `x`.
- `TEST-*` names the test artifact that must provide evidence.
- Every story completion gate requires its tests and its linked acceptance scenarios.

## Dependencies and delivery order

```text
T001-T004 Setup
  -> T005-T012 Foundation and pre-booking test gates
    -> US1 Availability
      -> US2 Booking
        -> US3 Own reservations/history
          -> US4 Room administration
            -> T038-T042 Polish and release evidence
```

US1 can begin after Foundation. US2 depends on the overlap, SQL Server concurrency,
ownership, and failed-update-preservation tests in Foundation. US3 depends on the
reservation contract and booking invariant from US2. US4 depends on the shared room
and authorization foundation, but does not modify reservation ownership rules.

## Parallelization rules

`[P]` is used only where tasks touch different files/contracts and have no dependency
on unfinished shared work. Tasks that edit the same contract, DbContext, migrations,
shared UI shell, or test fixture are intentionally sequential.

## Phase 1: Setup

- [X] T001 Create the monorepo solution and project manifests in `src/SmartSpace.Api/`, `src/SmartSpace.UI/`, `tests/SmartSpace.UnitTests/`, and `tests/SmartSpace.IntegrationTests/`; Depends: none; Done when all four project roots exist, build entrypoints are declared, and no application feature behavior is implemented.
- [X] T002 [P] Add repository-level development configuration and secret-handling placeholders in `.gitignore`, `README.md`, and `src/SmartSpace.Api/appsettings.Development.example.json`; Depends: T001; Done when SQL Server credentials are represented only by environment/user-secret placeholders and no secret value is committed.
- [X] T003 Confirm the constitution-aligned stack in `docs/decisions/ADR-001-stack-and-application-boundaries.md`; Depends: none; Done when .NET 10, ASP.NET Core Minimal API, standalone Blazor WebAssembly, EF Core 10, Tailwind CSS, and SQL Server are recorded consistently and no alternative frontend/API stack remains.
- [X] T004 [P] Add the local run configuration for API `http://localhost:5080` and Blazor UI `http://localhost:3000` in `src/SmartSpace.Api/Properties/launchSettings.json` and `src/SmartSpace.UI/Properties/launchSettings.json`; Depends: T001; Done when the exact URLs and `http://localhost:3000` CORS origin are documented without secrets.

## Phase 2: Foundation and pre-booking test gates

- [X] T005 Define domain entities and value rules in `src/SmartSpace.Api/Domain/Location.cs`, `src/SmartSpace.Api/Domain/Resource.cs`, `src/SmartSpace.Api/Domain/Reservation.cs`, and `src/SmartSpace.Api/Domain/ReservationStatus.cs`; Depends: T001, T003; Done when required fields, active/cancelled states, exclusive end semantics, and no-hard-delete lifecycle are represented without HTTP concerns.
- [X] T006 Configure `SmartSpaceDbContext` and entity mappings in `src/SmartSpace.Api/Data/SmartSpaceDbContext.cs` and `src/SmartSpace.Api/Data/Configurations/`; Depends: T005; Done when relationships, positive capacity, `End > Start`, version fields, and indexes needed for ownership/availability are mapped.
- [X] T007 Create the initial SQL Server EF migration in `src/SmartSpace.Api/Data/Migrations/`; Depends: T006; Done when the migration creates the documented relationships and constraints and can be applied to an isolated SQL Server database.
- [X] T008 Add DTOs, ProblemDetails codes, and the checked-in OpenAPI contract in `src/SmartSpace.Api/Contracts/` and `specs/001-meeting-room-reservations/contracts/openapi.yaml`; Depends: T001, T003; Done when create/update requests are distinct, version is required only for updates/cancel/deactivation, and documented 400/401/403/404/409/503 meanings are stable.
- [X] T009 [P] Write unit tests for invalid time ordering and exclusive boundary intervals in `tests/SmartSpace.UnitTests/BookingIntervalRulesTests.cs`; Depends: T005; Done when tests cover end-before-start, equal start/end, partial overlap, containment, identical intervals, and adjacent intervals, and fail against an intentionally missing rule implementation.
- [X] T010 [P] Write unit tests for ownership and role decisions in `tests/SmartSpace.UnitTests/AuthorizationRulesTests.cs`; Depends: T005; Done when tests cover owner-derived identity, employee denial of another employee's reservation, employee denial of administration, and administrator access to another employee's reservation.
- [X] T011 [P] Write SQL Server integration tests for concurrent overlapping creates in `tests/SmartSpace.IntegrationTests/Reservations/ConcurrentReservationTests.cs`; Depends: T006, T007; Done when at least 100 repeated request pairs use separate contexts/process-equivalents against the same SQL Server database and assert at most one success plus no overlapping active rows.
- [X] T012 [P] Write integration tests for failed update preservation in `tests/SmartSpace.IntegrationTests/Reservations/FailedUpdatePreservesOriginalTests.cs`; Depends: T006, T007; Done when an overlap, stale-version, and invalid-time rejection each prove the original reservation values/status remain unchanged.
- [X] T013 [P] Write unit tests for Europe/Amsterdam DST requirements in `tests/SmartSpace.UnitTests/TimeZoneRulesTests.cs`; Depends: T005; Done when nonexistent local times are rejected and duplicate local times require an explicit choice, with no implicit server-timezone fallback.
- [X] T014 Add API authentication/authorization test identities and environment guard in `src/SmartSpace.Api/Security/DevelopmentIdentityHandler.cs`, `src/SmartSpace.Api/Security/AuthorizationPolicies.cs`, and `tests/SmartSpace.IntegrationTests/Security/DevelopmentIdentityTests.cs`; Depends: T003, T008, T010; Done when demo identities work only in Development and startup/configuration rejects them outside Development.
- [X] T015 Add shared API exception-to-ProblemDetails mapping in `src/SmartSpace.Api/Infrastructure/ProblemDetailsMapping.cs` and `tests/SmartSpace.UnitTests/ProblemDetailsMappingTests.cs`; Depends: T008; Done when validation, ownership, overlap, stale version, policy conflict, and temporary persistence errors map to stable codes and safe Dutch-facing details.
- [X] T016 Create the SQL Server integration fixture and isolated database lifecycle in `tests/SmartSpace.IntegrationTests/Infrastructure/SqlServerFixture.cs`; Depends: T007; Done when migrations apply/rollback per isolated scenario, secrets are external, and parallel tests cannot share mutable data accidentally.

## Phase 3: User Story 1 - Beschikbare vergaderruimte vinden (P1)

**Goal**: Medewerkers can search active rooms by exact interval, location, and
minimum capacity without exposing owner data.

**Independent test**: `TEST-US1-BROWSER` covers valid results, overlap exclusion,
adjacent availability, empty results, invalid intervals, DST messages, and keyboard-
accessible filter/error states.

- [X] T017 [US1] Implement availability query and filter service in `src/SmartSpace.Api/Features/Rooms/RoomAvailabilityService.cs`; Depends: T005-T008, T009, T013; Done when only active rooms/locations matching the entire interval, location, and minimum capacity are returned and cancelled reservations do not block results (FR-001, FR-002, FR-011, FR-015; AC-US1.1-US1.4).
- [X] T018 [US1] Add rooms and availability Minimal API endpoints in `src/SmartSpace.Api/Features/Rooms/RoomsEndpoints.cs`; Depends: T017, T015; Done when DTO validation, ProblemDetails, authorization, and stable response shapes match `contracts/openapi.yaml` (FR-001, FR-002, FR-018).
- [X] T019 [P] [US1] Build the search page and accessible filter components in `src/SmartSpace.UI/Pages/Availability.razor` and `src/SmartSpace.UI/Components/RoomSearch/`; Depends: T004, T008; Done when labels, keyboard focus, loading, empty, invalid-time, network-error, and result states are specified and requests use the typed API client directly.
- [X] T020 [US1] Add browser coverage for availability in `tests/SmartSpace.IntegrationTests/Browser/availability.spec.ts`; Depends: T018, T019; Done when the browser scenarios map to AC-US1.1-US1.4 and cover 375/768/1440px without claiming implementation success before execution.
- [X] T021 [US1] Add API integration coverage for availability in `tests/SmartSpace.IntegrationTests/Rooms/AvailabilityEndpointTests.cs`; Depends: T018, T016; Done when active/inactive rooms, filters, overlap, adjacent intervals, empty result, and invalid input are asserted against SQL Server.

## Phase 4: User Story 2 - Vergaderruimte boeken (P1)

**Goal**: A medewerker can create exactly one own active booking while SQL Server
prevents conflicting concurrent bookings.

**Independent test**: `TEST-US2-BOOKING` combines unit boundary tests, ownership tests,
SQL Server concurrency tests, and browser booking/error scenarios.

- [ ] T022 [US2] Implement transactional reservation creation in `src/SmartSpace.Api/Features/Reservations/ReservationService.cs`; Depends: T009-T016, T017; Done when future-time validation, active-resource validation, server-derived owner, exclusive-end overlap, SQL Server transaction/invariant, and safe conflict mapping are enforced (FR-003-FR-007; AC-US2.1-US2.4).
- [ ] T023 [US2] Add reservation creation Minimal API endpoint in `src/SmartSpace.Api/Features/Reservations/ReservationEndpoints.cs`; Depends: T022, T008, T015; Done when POST returns the documented 201/location/DTO or stable 400/409/503 ProblemDetails without accepting owner or status from the request.
- [ ] T024 [US2] Add API tests for booking success, invalid time, inactive room, overlap, adjacent interval, ownership, and concurrent requests in `tests/SmartSpace.IntegrationTests/Reservations/CreateReservationTests.cs`; Depends: T023, T011, T012; Done when every AC-US2 scenario has a named test and database invariants are asserted.
- [ ] T025 [P] [US2] Build the booking form and conflict/error states in `src/SmartSpace.UI/Pages/ReservationEdit.razor` and `src/SmartSpace.UI/Components/Reservations/BookingForm.razor`; Depends: T019, T023; Done when form labels, disabled submit state, retained input after conflict, DST choice/error, and safe Dutch messages are specified.
- [ ] T026 [US2] Add browser coverage for booking in `tests/SmartSpace.IntegrationTests/Browser/booking.spec.ts`; Depends: T024, T025; Done when valid booking, unavailable room, adjacent interval, concurrent conflict presentation, invalid time, and keyboard flow map to AC-US2.1-US2.4.

## Phase 5: User Story 3 - Eigen reserveringen en historie beheren (P1)

**Goal**: Owners can view, change, and cancel their own reservations while history
and failed changes remain safe.

**Independent test**: `TEST-US3-OWNERSHIP-HISTORY` covers upcoming/past/cancelled
views, another-owner denial, successful update, failed-update preservation, cancel,
and proposed repeated-cancel behavior.

- [ ] T027 [US3] Implement owner-scoped reservation queries in `src/SmartSpace.Api/Features/Reservations/OwnReservationQueryService.cs`; Depends: T022-T024; Done when upcoming, past, and cancelled views are derived correctly, paged stably, and never reveal another owner's data (FR-012, FR-013; AC-US3.1-US3.2).
- [ ] T028 [US3] Add own-reservation GET endpoints in `src/SmartSpace.Api/Features/Reservations/ReservationEndpoints.cs`; Depends: T027, T008; Done when query/filter contracts, 401/404 privacy behavior, and typed-client expectations match OpenAPI.
- [ ] T029 [US3] Implement owner-scoped update with version and original-state preservation in `src/SmartSpace.Api/Features/Reservations/ReservationService.cs`; Depends: T022, T012; Done when only own future active reservations can change, overlap/invalid/DST/stale failures leave the original intact, and successful updates replace the version (FR-008, FR-009).
- [ ] T030 [US3] Implement owner-scoped cancellation and proposed idempotent repeat behavior in `src/SmartSpace.Api/Features/Reservations/ReservationService.cs`; Depends: T022, T029; Done when first cancellation preserves history/releases the interval, unauthorized cancellation is denied, and repeated cancellation follows the explicitly labeled lab proposal without hard delete (FR-010, FR-011, FR-019).
- [ ] T031 [US3] Add PUT/cancel API integration tests in `tests/SmartSpace.IntegrationTests/Reservations/UpdateCancelReservationTests.cs`; Depends: T028-T030, T012; Done when all AC-US3 scenarios, failed-update preservation, stale version, unauthorized access, history snapshot fields, unlimited owner history, administrator management rights, and repeat-cancel proposal are represented.
- [ ] T032 [P] [US3] Build the own-reservations pages and accessible status/action components in `src/SmartSpace.UI/Pages/MyReservations.razor` and `src/SmartSpace.UI/Components/Reservations/`; Depends: T028-T030; Done when tabs, mobile cards, status text, edit/cancel visibility, confirmation, loading/empty/error states, and typed client fetching are specified.
- [ ] T033 [US3] Add browser coverage for own reservations and history in `tests/SmartSpace.IntegrationTests/Browser/own-reservations.spec.ts`; Depends: T031, T032; Done when upcoming/past/cancelled, update success/failure, cancel, other-owner denial, keyboard operation, and responsive layouts map to AC-US3.1-US3.5.

## Phase 6: User Story 4 - Ruimtes beheren (P2)

**Goal**: Administrators can maintain rooms and manage other employees' reservations
under the confirmed release-1 policy.

**Independent test**: `TEST-US4-ROOM-ADMIN` covers valid create/update/deactivate,
validation errors, blocked deactivation, unauthorized employee access, and history
preservation.

- [ ] T034 [US4] Implement room administration service in `src/SmartSpace.Api/Features/Administration/AdminRoomService.cs`; Depends: T005-T008, T014, T022; Done when positive capacity/location/name validation, admin role checks, version checks, deactivation guard, no implicit cancellation, and no hard delete are enforced (FR-014-FR-016; AC-US4.1-US4.6).
- [ ] T035 [US4] Add administration Minimal API endpoints in `src/SmartSpace.Api/Features/Administration/AdminRoomEndpoints.cs`; Depends: T034, T015, T008; Done when create/update/deactivate/reactivate DTOs match OpenAPI, create has no version requirement, updates have version requirements, and 403/409 ProblemDetails are stable.
- [ ] T036 [US4] Add room administration integration tests in `tests/SmartSpace.IntegrationTests/Administration/AdminRoomTests.cs`; Depends: T035, T016; Done when validation, active/inactive visibility, blocked deactivation, version conflicts, employee denial, and reservation-preserving lifecycle are asserted.
- [ ] T037 [P] [US4] Build the admin rooms page and accessible form/table components in `src/SmartSpace.UI/Pages/AdminRooms.razor` and `src/SmartSpace.UI/Components/Admin/`; Depends: T035; Done when admin-only navigation, inline validation, blocked-deactivation explanation, loading/error states, and keyboard/responsive behavior are specified.
- [ ] T038 [US4] Add browser coverage for room administration in `tests/SmartSpace.IntegrationTests/Browser/admin-rooms.spec.ts`; Depends: T036, T037; Done when valid admin flow, invalid input, blocked deactivation, employee denial, and responsive keyboard flow map to AC-US4.1-US4.6.

## Phase 7: Polish and cross-cutting release evidence

- [ ] T039 [P] Add cross-cutting accessibility and responsive requirements evidence templates in `tests/SmartSpace.IntegrationTests/Browser/accessibility.spec.ts` and `docs/validation/accessibility.md`; Depends: T020, T026, T033, T038; Done when keyboard, labels, focus, text alternatives, 375/768/1440px, 200% zoom, and no-overflow criteria are listed without claiming they passed.
- [ ] T040 [P] Add API contract consistency review notes in `docs/validation/openapi-review.md`; Depends: T008, T018, T023, T028, T035; Done when every documented route has an owning feature, DTO, status-code mapping, and requirement/AC/test reference.
- [ ] T041 Update `README.md` with local start, SQL Server prerequisite, exact URLs, CORS origin, migrations, test groups, and known open decisions; Depends: T001-T004, T039-T040; Done when commands are documented without secrets and the training prototype is not described as production acceptance.
- [ ] T042 Resolve or explicitly gate open product decisions in `specs/001-meeting-room-reservations/spec.md`, `specs/001-meeting-room-reservations/plan.md`, and `docs/decisions/`; Depends: T027-T038; Done when all remaining open product decisions are either confirmed by BIDN or clearly blocking/deferred; retry/commit behavior, historical snapshots, retention/privacy, running-reservation boundary, and reactivation are already confirmed.
- [ ] T043 Run the full traceability review in `docs/validation/traceability.md`; Depends: T040-T042; Done when every FR-001-FR-020 has at least one AC reference, implementation task, and named test artifact, with unresolved proposals listed separately.

## Traceability matrix

| Requirement group | Acceptance criteria | Primary tasks | Test artifacts |
|---|---|---|---|
| FR-001-FR-002, FR-011, FR-015, FR-015a, FR-017-FR-018 | AC-US1.1-US1.4, AC-US4.3 | T017-T021, T034-T038 | `AvailabilityEndpointTests.cs`, `AdminRoomTests.cs`, `availability.spec.ts`, T013 |
| FR-003-FR-007 | AC-US2.1-US2.4 | T009-T016, T022-T026 | `BookingIntervalRulesTests.cs`, `ConcurrentReservationTests.cs`, `CreateReservationTests.cs`, `booking.spec.ts` |
| FR-008-FR-013, FR-019 | AC-US3.1-US3.5 | T012, T027-T033 | `FailedUpdatePreservesOriginalTests.cs`, `UpdateCancelReservationTests.cs`, `own-reservations.spec.ts` |
| FR-014-FR-016 | AC-US4.1-US4.6 | T010, T014, T034-T038 | `AuthorizationRulesTests.cs`, `AdminRoomTests.cs`, `admin-rooms.spec.ts` |
| FR-020 | All ACs and traceability gate | T040-T043 | `openapi-review.md`, `traceability.md` |

## MVP and implementation strategy

**MVP**: Foundation plus US1 availability and its API/browser evidence. US2 booking
cannot start until T009-T016 are complete, especially boundary, SQL Server
concurrency, ownership, and failed-update-preservation tests.

Deliver incrementally per story. Complete each story's API, UI, integration tests,
browser tests, and independent acceptance evidence before starting the next story.
Do not treat unchecked requirements-review items or `Voorgesteld` ADRs as silently
approved implementation decisions.
