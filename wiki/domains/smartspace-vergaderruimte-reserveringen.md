---
title: SmartSpace – vergaderruimte-reserveringen
created: 2026-09-30
updated: 2026-09-30
type: domain
tags: [reservations, room-management, scope]
sources: [raw/projectgoals.md]
confidence: high
contested: false
contradictions: []
---

# SmartSpace – vergaderruimte-reserveringen

SmartSpace is de trainingscasus waarin BIDN centraal **vergaderruimtes** wil
reserveren. De opdracht levert bruikbare input voor een mogelijke echte
realisatie: een productspecificatie, systeemontwerp, backlog, lokaal technisch
prototype en architectuurbesluiten. Een trainingsdemo is geen productieacceptatie.

## Release 1

Release 1 ondersteunt **vergaderruimtes** en **ruimtebeheer**. Medewerkers kunnen:

- beschikbaarheid bekijken
- boeken
- wijzigen
- annuleren
- eigen reserveringen inclusief historie raadplegen

De centrale, oorspronkelijke businessregel is dat een ruimte nooit twee actieve
overlappende boekingen mag hebben; zie [[concepts/boekingsintegriteit]].

## Scope en afbakening

Wat wel en niet tot Release 1 hoort, staat in [[concepts/scope-release-1]]. Het
ontwerp moet latere uitbreiding mogelijk maken zonder die nu te bouwen.

## Validatie

Het prototype draait lokaal met synthetische data. Een productverantwoordelijke
valideert productregels en scope; een ontwikkelaar reviewt ontwerp, beveiliging,
code en testbewijs. Oorspronkelijke businessregels worden onderscheiden van nog
niet goedgekeurde labvoorstellen.
