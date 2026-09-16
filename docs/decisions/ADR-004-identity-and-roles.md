# ADR-004: Identiteit en rollen

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

Eigen reserveringen vereisen een betrouwbare server-side identiteit. Een beheerder
beheert in release 1 alleen ruimtes en geen reserveringen van anderen.

## Besluit

Leid de eigenaar af uit gevalideerde claims. Gebruik een development-only identity
alleen in Development; buiten Development moet de API weigeren op te starten als
demo-auth actief staat. Handhaaf medewerker- en beheerdersrechten in de API.

## Alternatieven

- Een userId uit de request-body.
- Een client-header of keuzeveld met gebruikersnaam.
- Beheerder toegang geven tot alle boekingen.

## Motivatie

Server-side claims en minimale bevoegdheden beschermen eigenaarschap en privacy.

## Consequenties

De identity-integratie en formele BIDN-roltoewijzing blijven vóór productie een
open besluit. Negatieve autorisatietests zijn verplicht.
