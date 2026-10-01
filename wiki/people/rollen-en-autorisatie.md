---
title: Rollen en autorisatie
created: 2026-09-30
updated: 2026-09-30
type: person
tags: [governance, reservations, room-management]
sources: [raw/appfeatures.md, raw/techstack.md]
confidence: high
contested: false
contradictions: []
---

# Rollen en autorisatie

SmartSpace kent twee rollen: **medewerker** en **beheerder**. Autorisatie wordt
op elk endpoint en in de mutatielogica afgedwongen; een verborgen knop is
onvoldoende. De regels hieronder haken in op [[concepts/businessregels]] en het
domein in [[domains/smartspace-vergaderruimte-reserveringen]].

## Medewerker

- Ruimtes bekijken met naam, capaciteit en locatie. ^[raw/appfeatures.md]
- Vrije ruimtes zoeken voor een gekozen begin- en eindtijd. ^[raw/appfeatures.md]
- Een ruimte voor zichzelf reserveren. ^[raw/appfeatures.md]
- Eigen toekomstige actieve reservering wijzigen of annuleren. ^[raw/appfeatures.md]
- Eigen toekomstige, afgelopen en geannuleerde reserveringen bekijken. ^[raw/appfeatures.md]

## Beheerder

- Ruimte toevoegen; naam, positieve capaciteit en locatie aanpassen. ^[raw/appfeatures.md]
- Ruimte deactiveren wanneer geen actieve lopende of toekomstige reserveringen
  meer bestaan. ^[raw/appfeatures.md]
- **Geen** extra bevoegdheden over boekingen van andere medewerkers. De
  beheerdersrol geeft dus niet automatisch toegang tot alle
  reserveringsdetails. ^[raw/appfeatures.md]

## Autorisatiematrix

| Handeling | Medewerker | Beheerder |
| --- | --- | --- |
| Ruimtes en beschikbaarheid bekijken | Ja | Ja |
| Eigen reservering aanmaken/bekijken/wijzigen/annuleren | Ja | Ja |
| Reservering van een ander inzien of beheren | Nee | Niet standaard; apart beleidsbesluit |
| Ruimtes, capaciteit en locatie configureren | Nee | Ja |

^[raw/techstack.md]

## Identiteit

`OwnerSubjectId` wordt afgeleid uit server-side gevalideerde claims, nooit uit
een vrij invulbaar `userId` in de request-body. Voor de training geldt een
development-only authenticatiehandler met vaste testpersonen (minimaal twee
medewerkers en één beheerder); buiten Development moet startup weigeren als
demo-authenticatie aanstaat. Entra ID is een voorstel voor bedrijfsgebruik en
nog geen goedgekeurd besluit. ^[raw/techstack.md]

## Open besluit

Of een beheerder boekingen van anderen mag beheren, en welke details collega's
zien, is een expliciet open besluit voor bedrijfsgebruik.
