# Implementation Plan: SmartSpace release 1 vergaderruimtes

**Branch**: `001-meeting-room-reservations` | **Date**: 2026-09-16 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-meeting-room-reservations/spec.md`

## Summary

SmartSpace release 1 ondersteunt het zoeken, boeken, wijzigen, annuleren en
raadplegen van eigen reserveringen voor vergaderruimtes, plus ruimtebeheer door
beheerders. De monorepo gebruikt .NET 10 C#, ASP.NET Core Minimal API, EF Core 10
met SQL Server migrations en standalone Blazor WebAssembly.

De reserveringsregels worden server-side en transactioneel afgedwongen. De Blazor-
frontend praat rechtstreeks met de API via een exacte Development-CORS-origin en
gebruikt een getypeerde HttpClient voor gebruikersafhankelijke boekingsdata. Geen
applicatiecode wordt in deze planningsfase gemaakt.

## Technical Context

**Language/Version**: C#/.NET 10

**Primary Dependencies**: ASP.NET Core Minimal API, Entity Framework Core 10, SQL
Server provider, standalone Blazor WebAssembly, Tailwind CSS, OpenAPI/ProblemDetails

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
| Vastgelegde stack en eenvoudigste architectuur | PASS | De planstack volgt .NET 10, Blazor WebAssembly, Minimal API, EF Core, Tailwind CSS en SQL Server. |
| Foutscenario's en geautomatiseerde tests | PASS | Unit-, SQL Server-integratie- en browserflows bevatten positieve en negatieve paden. |
| Secrets en persoonsgegevens | PASS | Configuratie gebruikt environment settings; ProblemDetails/logging minimaliseren data. |
| Toegankelijke UI | PASS | Keyboard, labels, focus, responsive states en begrijpelijke foutmeldingen zijn gates. |
| Voorstelstatus expliciet | PASS | Alle nieuwe architectuurkeuzes staan in `docs/decisions` met status Voorgesteld. |

**Gate-uitkomst vóór research**: PASS voor stackconsistentie. Open productbesluiten
blijven afzonderlijke pre-implementation gates; deze planning keurt geen product- of
architectuurbesluit buiten de constitution goed.

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
└── SmartSpace.UI/
  ├── Pages/
  ├── Components/
  ├── Layout/
  ├── Services/
  └── Styles/

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
- De Minimal API-, SQL Server-migrations- en Blazor-keuzes worden getoetst aan de
  constitutionele stackregel.

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

**Status**: PASS voor constitutionele stackconsistentie. Historische room-snapshots,
idempotente herhaalde annulering en de exacte grens van een "lopende" reservering
blijven expliciete productbesluiten of gelabelde voorstellen. Geen implementatie of
productgoedkeuring volgt uit deze planfase.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|---|---|---|
| Twee aparte integration-testprojecten | SQL Server-concurrency en browserflows hebben verschillende runtime-eisen. | Eén testproject zou verantwoordelijkheden en testfixtures onnodig vermengen. |
