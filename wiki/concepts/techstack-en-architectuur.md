---
title: Techstack en architectuur
created: 2026-09-30
updated: 2026-09-30
type: concept
tags: [lab-proposal, governance, scope]
sources: [raw/techstack.md, raw/projectgoals.md]
confidence: high
contested: false
contradictions: []
---

# Techstack en architectuur

Ontwerpinput voor het SmartSpace-trainingsprototype. De keuzes hieronder zijn
ontwerpkeuzes en nog geen door BIDN goedgekeurde besluiten. Zie het domein in
[[domains/smartspace-vergaderruimte-reserveringen]] en de regels in
[[concepts/businessregels]].

## Techstack

- Framework: .NET 10 (`net10.0`) voor alle C#-projecten.
- Frontend: standalone Blazor WebAssembly, gestyled met Tailwind CSS 4 via lokale CLI.
- Backend: ASP.NET Core 10 Minimal API (geen controllers).
- Data access: Entity Framework Core 10.
- Database prototype: SQLite in-memory (`Cache=Shared`, keeper-connection).
- Contracten: gedeelde C# DTO-library met stabiele foutcodes, zonder EF.
- Identiteit prototype: development-only testidentiteit; Entra ID is een voorstel.
- Verificatie: xUnit en ASP.NET Core-integratietests. ^[raw/techstack.md]

## Architectuur

De Blazor-frontend roept de Minimal API aan via een getypeerde `HttpClient`.
Endpoints verwerken DTO's en delegeren naar services; services handhaven
businessregels en gebruiken een scoped EF Core `DbContext`. De browser benadert
de database nooit rechtstreeks. Reserveringslogica blijft buiten `Program.cs` en
gericht testbaar. Geen microservices, generieke repositorylaag of verplichte
CQRS/MediatR voor deze scope. ^[raw/techstack.md]

## Kritisch onderscheid: prototype versus bedrijfsrelease

SQLite in-memory verliest alle gegevens zodra het proces stopt. De eis
"historische reserveringen blijven raadpleegbaar" is daarmee alleen binnen één
prototypesessie aantoonbaar. Voor een echte bedrijfsrelease is duurzame opslag
noodzakelijk. Een overstap naar SQL Server/Azure SQL vraagt provider-specifieke
migrations en opnieuw geteste concurrency; alleen de connection string wijzigen
is onvoldoende. ^[raw/techstack.md]

## Concurrency en tijdzone

Eén immediate SQLite-write-transactie omvat het hele traject (lock verkrijgen,
lezen, controleren, opslaan, committen). `SQLITE_BUSY`/`SQLITE_LOCKED` volgt
begrensde retry; na uitputting 503, bij overlap 409. Een applicatiebeheerd
`Version`-token (Guid) verhindert overschrijven door een verouderd formulier.
Tijden worden als UTC opgeslagen; Europe/Amsterdam is de weergavetijdzone en
`TimeProvider` wordt geïnjecteerd voor reproduceerbare tests. ^[raw/techstack.md]

## Architectuurbesluiten

TechStack verwijst naar ADR-001 (stack/grenzen), ADR-002 (tijdelijke vs
duurzame opslag), ADR-003 (overlap en transacties), ADR-004 (identiteit/rollen),
ADR-005 (tijdzone) en ADR-006 (Resource-model en toekomstige werkplekken). Deze
onderbouwen de afbakening in [[concepts/scope-release-1]]. ^[raw/techstack.md]
