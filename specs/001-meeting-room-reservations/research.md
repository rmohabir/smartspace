# Research: SmartSpace release 1

## Scope and unresolved policy

**Decision**: Houd release 1 beperkt tot vergaderruimtes, eigen reserveringen en
ruimtebeheer. Behoud de bevestigde regels voor eigenaarschap, exclusieve eindgrens
en Europe/Amsterdam-zomertijd.

**Rationale**: Dit volgt de actieve spec en voorkomt dat ontbrekende
`OpenQuestions.md`-input als BIDN-beleid wordt ingevuld.

**Alternatives considered**: Werkplekken, terugkerende reserveringen, notificaties,
deelnemers en externe integraties zijn uitgesteld naar buiten scope.

## Persistente opslag

**Decision**: SQL Server is de persistente bron van waarheid en EF Core migrations
zijn onderdeel van de implementatie.

**Rationale**: Dit volgt de constitution en maakt historie, herstel en
multi-process-concurrency toetsbaar.

**Alternatives considered**: SQLite in-memory is alleen geschikt voor het eerdere
trainingsprototype en wordt niet als release-1 persistentie gebruikt; SQLite op
schijf en Azure SQL blijven niet gekozen alternatieven totdat BIDN dat besluit.

## API-boundary

**Decision**: Planinput kiest ASP.NET Core controller Web API met DTO's,
ProblemDetails en OpenAPI.

**Rationale**: Controllers geven een expliciete HTTP-boundary en ondersteunen het
gevraagde contractdocument.

**Alternatives considered**: De StakeholderDocuments/TechStack.md beschrijft
Minimal API. Dat is niet stilzwijgend behouden of vervangen; de controllerkeuze
blijft **Voorgesteld** en vereist constitutionele afstemming.

## Frontend-boundary

**Decision**: Planinput kiest React met Next.js App Router en TypeScript. Interactieve
boekingsdata gebruikt client components en `no-store`; browserrequests gaan
rechtstreeks naar `http://localhost:5080` vanuit de exacte origin
`http://localhost:3000`.

**Rationale**: Dit volgt de expliciete actuele planinput en houdt gebruikers-
afhankelijke gegevens uit statische/cached serverweergaven.

**Alternatives considered**: De stakeholder-TechStack beschrijft standalone Blazor
WebAssembly met Tailwind. Dit blijft een constitutionele afwijking in voorgestelde
status. Een server-side proxy is niet gekozen omdat de browser rechtstreeks de API
moet benaderen.

## Booking integrity

**Decision**: Gebruik halfopen tijdvakken `[start, end)`. Controleer overlap binnen
een transactionele SQL Server-mutatie; recheck na database-conflict en laat de
invariant door de database/transactionele isolatie bewaken.

**Rationale**: Hierdoor zijn 10:00-11:00 en 11:00-12:00 aansluitend toegestaan en
kan geen dubbele actieve reservering ontstaan bij meerdere API-processen.

**Alternatives considered**: Een voorafgaande availability-check zonder transactie,
process-local locks en alleen een concurrencytoken zijn onvoldoende.

## Local time handling

**Decision**: Europe/Amsterdam is de invoer- en weergavetijdzone. Niet-bestaande
lokale tijden worden geweigerd; dubbel voorkomende tijden vragen expliciete keuze.
Opslag en contractnormalisatie worden als instant/tijd-offset behandeld.

**Rationale**: Dit voorkomt stilzwijgende verschuiving tijdens zomer-/wintertijd.

**Alternatives considered**: Automatische correctie, automatisch de eerste dubbele
tijd kiezen of alleen UTC vragen zijn afgewezen wegens gebruikersrisico.

## Test strategy

**Decision**: Unit tests voor pure regels, SQL Server-integratietests voor migrations
en concurrency, en browsertests voor end-to-end gebruikersflows en toegankelijkheid.

**Rationale**: De constitution vereist bewijs op de echte persistence boundary en
feature-level failure coverage.

**Alternatives considered**: Alleen mocks of alleen browserchecks zijn onvoldoende
voor ownership, transactionele integriteit en database-invarianten.

## Architecture decision status

De keuzes in dit document zijn planinput, geen BIDN-goedkeuring. De stackafwijking
van de constitution moet vóór implementatie worden bevestigd via de documenten in
`docs/decisions/` en een constitutionele amendment indien nodig.
