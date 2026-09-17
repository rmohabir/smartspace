# Quickstart validation: SmartSpace release 1

Dit is een validatiegids voor de geplande implementatie. De commando's zijn
richtlijnen voor de toekomstige monorepo en voeren in deze planfase niets uit.

## Prerequisites

- .NET 10 SDK
- Node.js LTS en npm
- SQL Server voor lokale ontwikkeling of een geïsoleerde testinstance
- Browser met keyboard- en responsive inspection
- Repository checkout op branch `001-meeting-room-reservations`

## Expected project configuration

- API: `http://localhost:5080`
- Webapp: `http://localhost:3000`
- Exacte Development-CORS-origin: `http://localhost:3000`
- SQL Server connection string via environment/user secrets, nooit in Git

## Start sequence

1. Apply EF Core migrations to the isolated SQL Server database.
2. Start the API on `http://localhost:5080`.
3. Start the Blazor WebAssembly UI on `http://localhost:3000`.
4. Confirm that browser requests go directly from the webapp origin to the API.
5. Open the generated OpenAPI document and compare it with
   [contracts/openapi.yaml](contracts/openapi.yaml).

The exact package-manager and migration commands belong in implementation tasks
once the proposed stack is approved; this document must not contain implementation
code or secrets.

## Validation scenarios

### Availability

- Search a valid future interval with location and minimum-capacity filters.
- Verify partial, enclosing and identical overlaps are unavailable.
- Verify an interval ending exactly at the requested start is available.
- Verify inactive rooms and inactive locations are excluded from new results.

### Booking and concurrency

- Create a valid booking and find it in the owner's upcoming list.
- Submit 100 repeated pairs of concurrent overlapping requests for one room.
- Assert at most one success per pair and no overlapping active reservations in SQL
  Server after all requests finish.
- Verify a rejected conflict returns a stable ProblemDetails code.

### Change and cancellation

- Update an owner's future booking to a free room/time.
- Attempt an update into an occupied interval; assert conflict and unchanged source
  reservation.
- Cancel an own future booking; assert history remains and the interval is free.
- Attempt read/change/cancel with another employee's identity; assert denial without
  leaking owner data. Repeat with an administrator identity and assert the confirmed
  administrator permission is applied.

### Room administration

- Create a location with a name and optional building/floor; verify it appears in
  the room location select.
- Edit a location and verify its rooms remain linked.
- Create and update a room by selecting a readable location name; no location ID
  should be typed manually.
- Reject empty room name, non-positive capacity and no selected location.
- Deactivate a room without current/future active reservations.
- Reject deactivation when such reservations exist; do not cancel them implicitly.
- Attempt administration as a regular employee and assert denial.
- Attempt location create/update as a regular employee and assert denial.

### Time and accessibility

- Reject a nonexistent Europe/Amsterdam DST local time.
- Require an explicit choice for a duplicated local time.
- Exercise all supported workflows with keyboard only.
- Check visible labels, focus, text error messages, responsive layouts at 375, 768
  and 1440 pixels, and no horizontal overflow.

## Evidence to retain

- Test report with requirement and scenario IDs.
- SQL Server migration and concurrency evidence.
- Browser screenshots or test output for responsive/accessibility checks.
- OpenAPI contract validation result.
- Decision review confirming all `Voorgesteld` ADRs and any constitution amendment.
