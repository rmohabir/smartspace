---
title: Boekingsintegriteit – geen overlappende boekingen
created: 2026-09-30
updated: 2026-09-30
type: concept
tags: [business-rules, reservations]
sources: [raw/projectgoals.md]
confidence: high
contested: false
contradictions: []
---

# Boekingsintegriteit – geen overlappende boekingen

Een vergaderruimte mag **nooit twee actieve overlappende boekingen** hebben. Dit
is een oorspronkelijke businessregel van SmartSpace, inclusief bescherming tegen
gelijktijdige aanvragen.

## Oorspronkelijke regel versus labvoorstel

De regel zelf (geen overlap, ook bij concurrency) is goedgekeurde
stakeholderinput. De **technische keuzes** om dit af te dwingen — bijvoorbeeld
transactietechniek, opslag of vergrendeling — zijn nog labvoorstellen en geen
goedgekeurde besluiten. Verwante nog-niet-goedgekeurde voorstellen zijn onder
meer boekingshorizon, buffers, beheerrechten, identiteit en opslag.

## Verband

Deze regel is de kern van [[domains/smartspace-vergaderruimte-reserveringen]] en
werkt door in wat wel/niet binnen [[concepts/scope-release-1]] valt.
