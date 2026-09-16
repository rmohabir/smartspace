# Implementation Plan: SmartSpace release 1 vergaderruimtes

**Branch**: `001-meeting-room-reservations` | **Date**: 2026-09-16 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-meeting-room-reservations/spec.md`

## Summary

SmartSpace release 1 ondersteunt het zoeken, boeken, wijzigen, annuleren en
raadplegen van eigen reserveringen voor vergaderruimtes, plus ruimtebeheer door
beheerders. De planinput vraagt een monorepo met een .NET 10 C# controller Web API,
EF Core 10 met SQL Server migrations, en een React/Next.js App Router frontend.

De reserveringsregels worden server-side en transactioneel afgedwongen. De browser
praat rechtstreeks met de API via een exacte Development-CORS-origin. Interactieve,
gebruikersafhankelijke boekingsdata wordt in client components met `no-store`
opgehaald. Geen applicatiecode wordt in deze planningsfase gemaakt.

## Technical Context

**Language/Version**: C#/.NET 10; TypeScript volgens de gekozen Next.js-toolchain

**Primary Dependencies**: ASP.NET Core controller Web API, Entity Framework Core 10,
SQL Server provider, React, Next.js App Router, accessible responsive UI components,
OpenAPI/ProblemDetails

**Storage**: SQL Server als persistente bron van waarheid; EF Core migrations

**Testing**: .NET unit tests, SQL Server integration tests, browser end-to-end tests
met een echte API/database testomgeving

**Target Platform**: Lokale ontwikkeling op Windows; API op `http://localhost:5080`;
webapp op `http://localhost:3000`

**Project Type**: Monorepo web application met backend API, frontend en twee
geautomatiseerde testprojecten

**Performance Goals**: Beschikbaarheids- en eigen-reserveringsqueries geven onder
normale trainingsbelasting binnen 2 seconden een bruikbaar resultaat; 100 herhaalde
concurrente aanvraagparen mogen nooit twee overlappende actieve reserveringen
opleveren

**Constraints**: Exacte Development-CORS-origin `http://localhost:3000`; browser
benadert de API rechtstreeks; geen secrets in broncode of logs; Europe/Amsterdam
voor invoer en weergave; niet-bestaande lokale tijden worden geweigerd en dubbele
lokale tijden vragen een expliciete keuze; geen hard delete van reserveringen of
gerefereerde ruimtes; historische room-snapshot versus actuele roomgegevens is nog
een open productbesluit

**Scale/Scope**: Release 1 voor BIDN-vergaderruimtes, medewerkers en beheerders;
geen werkplekken als gebruikersfunctie, herhaalboekingen, notificaties,
deelnemerslijsten, check-in/check-out of externe integraties

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle / gate | Status | Plan response |
|---|---|---|
| Traceerbaar businessgedrag | PASS | FR's, stories, scenario's, contracten en tests worden per feature gekoppeld. |
| SQL Server als persistente bron | PASS | SQL Server, migrations en echte SQL Server-concurrencytests zijn leidend. |
| Backend-autorisatie en eigenaarschap | PASS | Claims, policies en eigenaarchecks zitten in de API en worden getest. |
| Development-identiteit buiten Development uit | PASS | De development identity wordt environment-gated en negatief getest. |
| Historie en geen hard delete | PASS | Annuleren is een statuswijziging; rooms en reservations worden niet verwijderd. |
| Vastgelegde stack en eenvoudigste architectuur | CONDITIONAL | De planinput wijkt af van de constitutionele Blazor/Minimal API-keuze naar Next.js/React en controller Web API. Dit blijft Voorgesteld totdat de constitution wordt gewijzigd of de stackkeuze wordt teruggedraaid. |
| Foutscenario's en geautomatiseerde tests | PASS | Unit-, SQL Server-integratie- en browserflows bevatten positieve en negatieve paden. |
| Secrets en persoonsgegevens | PASS | Configuratie gebruikt environment settings; ProblemDetails/logging minimaliseren data. |
| Toegankelijke UI | PASS | Keyboard, labels, focus, responsive states en begrijpelijke foutmeldingen zijn gates. |
| Voorstelstatus expliciet | PASS | Alle nieuwe architectuurkeuzes staan in `docs/decisions` met status Voorgesteld. |

**Gate-uitkomst vóór research**: CONDITIONAL, niet volledig PASS. Er wordt geen
implementatie gestart. Goedkeuring van de stackafwijking of een constitutionele
amendment is een pre-implementation besluit.

## Project Structure

### Documentation (this feature)

```text
specs/001-meeting-room-reservations/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/openapi.yaml
├── checklists/requirements.md
└── tasks.md                 # later via /speckit-tasks
```

### Source Code (repository root)

```text
src/
├── SmartSpace.Api/
│   ├── Features/
│   │   ├── Rooms/
│   │   ├── Reservations/
│   │   └── Administration/
│   ├── Data/
│   │   ├── SmartSpaceDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Domain/
│   ├── Security/
│   └── Program.cs
└── smartspace-web/
    ├── app/
    ├── components/
    ├── lib/
    ├── public/
    └── styles/

tests/
├── SmartSpace.UnitTests/
└── SmartSpace.IntegrationTests/

docs/decisions/
├── ADR-001-stack-and-application-boundaries.md
├── ADR-002-sql-server-persistence.md
├── ADR-003-booking-integrity.md
├── ADR-004-identity-and-roles.md
├── ADR-005-timezone-policy.md
└── ADR-006-history-and-room-lifecycle.md
```

**Structure Decision**: Een monorepo met één API, één webapp en gescheiden unit- en
integratietestprojecten. Backendcode blijft per feature georganiseerd; `Data` bevat
alleen DbContext, mappings en migrations. DTO's zijn onderdeel van featurecontracten
en EF-entiteiten lekken niet naar de HTTP-contracten.

## Phase 0: Research Outputs

- `research.md` legt gekozen, voorgestelde en afgewezen alternatieven vast.
- Open punten uit de spec worden niet als BIDN-beleid ingevuld.
- De controller Web API-, SQL Server-migrations- en React/Next.js-keuzes worden
  getoetst aan de constitutionele stackregel.

## Phase 1: Design Outputs

- `data-model.md`: entiteiten, relaties, statussen, validatie en historiebeleid.
- `contracts/openapi.yaml`: publieke API-contracten met DTO's, statuscodes en
  ProblemDetails.
- `quickstart.md`: lokale run- en validatiescenario's zonder implementatiecode.
- `docs/decisions/*`: rationale, alternatieven, consequenties en status
  **Voorgesteld**.

## Test Strategy

1. **Unit tests**: tijdvakvalidatie, halfopen overlap, statusovergangen, ownership-
   beslissingen, DTO-validatie en ProblemDetails-mapping.
2. **SQL Server integration tests**: migrations, foreign keys, persistence,
  cancellation/history, voorgestelde idempotente herhaalde annulering,
  deactivation guards en minimaal 100 paren gelijktijdige
   create requests met afzonderlijke API-processen of equivalent geïsoleerde
   processen. Assert responses én database-invariant.
3. **Browser tests**: zoeken, filteren, boeken, wijziging met conflict, annuleren,
   eigen historie, onbevoegd beheer, beheerflow, foutmeldingen, keyboardbediening,
   375/768/1440 px en zomertijdgevallen.

## Post-Design Constitution Re-check

**Status**: CONDITIONAL blijft staan totdat de voorgestelde frontend/API-stack als
constitutionele wijziging is goedgekeurd. De overige gates zijn ontwerpbaar conform
de constitution. Historische room-snapshots, idempotente herhaalde annulering en de
exacte grens van een "lopende" reservering blijven expliciete productbesluiten of
gelabelde voorstellen. Geen implementatie of productgoedkeuring volgt uit deze
planfase.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|---|---|---|
| Frontend/API-stack wijkt af van constitution | De planinput vraagt React/Next.js en controller Web API. | De constitutionele Blazor/Minimal API-stack is niet stilzwijgend vervangen; de afwijking blijft voorgesteld en vereist expliciete amendment. |
| Twee aparte integration-testprojecten | SQL Server-concurrency en browserflows hebben verschillende runtime-eisen. | Eén testproject zou verantwoordelijkheden en testfixtures onnodig vermengen. |
