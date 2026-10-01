---
title: Businessregels reserveringen
created: 2026-09-30
updated: 2026-09-30
type: concept
tags: [business-rules, reservations, lab-proposal]
sources: [raw/appfeatures.md, raw/techstack.md]
confidence: high
contested: false
contradictions: []
---

# Businessregels reserveringen

Deze pagina consolideert de businessregels voor SmartSpace en scheidt
**oorspronkelijke eisen** van **labvoorstellen** die BIDN nog moet bevestigen. De
kernregel (geen overlappende actieve boekingen) staat apart in
[[concepts/boekingsintegriteit]]; autorisatie staat in
[[people/rollen-en-autorisatie]].

## Vaststaande businessregels (opgegeven eis)

- Een reservering heeft een begin- en eindtijd; het einde ligt ná het begin. ^[raw/appfeatures.md]
- Actieve reserveringen voor dezelfde ruimte mogen niet overlappen; de
  overlapcontrole geldt zowel bij aanmaken als bij wijzigen. ^[raw/techstack.md]
- Ook twee gelijktijdige aanvragen mogen geen dubbele boeking opleveren. ^[raw/appfeatures.md]
- Aansluitende boekingen zijn toegestaan (bijv. 10–11 en 11–12); overlappende
  actieve boekingen niet. ^[raw/appfeatures.md]
- Een afgewezen wijziging laat de bestaande reservering intact. ^[raw/appfeatures.md]
- Annuleren bewaart de historie, verwijdert de rij niet en geeft het tijdvak
  weer vrij. ^[raw/techstack.md]
- Historische en geannuleerde reserveringen blijven voor de eigenaar
  raadpleegbaar en worden niet fysiek verwijderd. ^[raw/appfeatures.md]
- Andere medewerkers zien geen persoonsgegevens in beschikbaarheid. ^[raw/appfeatures.md]
- De lokale UI toont tijden voor Europe/Amsterdam. ^[raw/appfeatures.md]

## Regel-ID's uit TechStack §6

TechStack.md nummert de regels als BR-01 t/m BR-10 met een expliciete status
(opgegeven eis, uitwerking of voorstel):

- BR-01 begin/eind met eind na begin — opgegeven eis.
- BR-02 geen overlap voor dezelfde resource — opgegeven eis.
- BR-03 overlapcontrole bij aanmaken én wijzigen — uitwerking van eis.
- BR-04 geannuleerde reserveringen blokkeren geen beschikbaarheid — ontwerpvoorstel.
- BR-05 historische/geannuleerde reserveringen niet fysiek verwijderen — uitwerking historie-eis.
- BR-06 aansluitende boekingen toegestaan, geen buffer — ontwerpvoorstel.
- BR-07 medewerkers wijzigen/annuleren alleen eigen reserveringen — autorisatievoorstel.
- BR-08 nieuwe reserveringen starten in de toekomst; begonnen/afgelopen zijn alleen-lezen — beleidsvoorstel, te valideren.
- BR-09 alleen actieve ruimtes op actieve locaties zijn nieuw te reserveren — ontwerpvoorstel.
- BR-10 deactiveren met toekomstige actieve boekingen wordt geweigerd — beleidsvoorstel, geen stilzwijgende annulering. ^[raw/techstack.md]

## Labvoorstellen en te bevestigen uitwerkingen

De volgende punten zijn nog géén goedgekeurd beleid en vragen bevestiging door
BIDN:

- Beleid voor lopende of direct aansluitende boekingen bij de "start in de
  toekomst"-regel. ^[raw/appfeatures.md]
- Een eventuele buffer tussen aansluitende boekingen. ^[raw/appfeatures.md]
- De precieze beheerprocedure en bevoegdheden bij deactiveren. ^[raw/appfeatures.md]
- Standaard geen minimale/maximale duur, boekingshorizon of openingstijden
  totdat BIDN die vaststelt. ^[raw/techstack.md]

## Overlapdefinitie

Gebruik halfopen intervallen `[start, end)`. Twee reserveringen overlappen
wanneer `existing.StartUtc < requestedEndUtc && existing.EndUtc >
requestedStartUtc`, gefilterd op dezelfde `ResourceId` en status `Active` (bij
wijzigen de eigen reservering uitgesloten). ^[raw/techstack.md]
