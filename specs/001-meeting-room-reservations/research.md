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

**Decision**: SmartSpace gebruikt ASP.NET Core Minimal API met DTO's,
ProblemDetails en OpenAPI.

**Rationale**: Minimal API past bij de constitutionele stack en ondersteunt het
gevraagde contractdocument.

**Alternatives considered**: Een controllergebaseerde API is niet gekozen omdat die
niet in de vastgelegde constitutionele stack staat.

## Frontend-boundary

**Decision**: SmartSpace gebruikt standalone Blazor WebAssembly met Tailwind CSS.
De UI gebruikt typed HttpClient-calls rechtstreeks naar `http://localhost:5080`
vanuit de exacte Development-origin `http://localhost:3000`.

afhankelijke gegevens uit statische/cached serverweergaven.
**Rationale**: Dit volgt de constitutionele stack en de stakeholder-TechStack.

**Alternatives considered**: Een moderne JavaScript-frontend is niet gekozen omdat
de constitutionele stack Blazor WebAssembly voorschrijft.
Een server-side proxy is niet gekozen omdat de browser rechtstreeks de API moet
benaderen.

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

## Location administration UX

**Decision**: Roombeheer gebruikt een select met actieve locatienamen en optionele
gebouwinformatie. Locatie toevoegen en wijzigen gebeurt in een aparte sectie op
dezelfde adminpagina; locatie-ID's blijven uitsluitend API-identifiers.

**Rationale**: Een beheerder denkt in herkenbare plaatsnamen, niet in GUID's. Een
select voorkomt ongeldige relaties en maakt nieuwe locaties onmiddellijk bruikbaar
voor rooms.

**Alternatives considered**: Vrije GUID-invoer is afgewezen als foutgevoelig en
ontoegankelijk. Een losse locatiepagina zonder terugkoppeling naar roombeheer is
uitgesteld omdat de beheerder de nieuwe locatie direct bij roombewerking nodig heeft.
