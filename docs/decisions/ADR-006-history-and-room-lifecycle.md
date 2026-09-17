# ADR-006: Historie en levenscyclus van ruimtes

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

Annuleren moet historie behouden. Ruimtes kunnen later worden gedeactiveerd terwijl
oude reserveringen naar die ruimte moeten blijven verwijzen.
Locaties zijn beheerde catalogusgegevens; rooms moeten aan een leesbare locatie
worden gekoppeld zonder dat een beheerder een GUID hoeft te kennen.

## Besluit

Annuleren zet een reservering op `Cancelled` en verwijdert haar niet. Een gebruikte
ruimte wordt gedeactiveerd in plaats van hard verwijderd. Deactiveren is verboden bij
lopende of toekomstige actieve reserveringen en annuleert die niet stilzwijgend.
Beheerders kunnen locaties toevoegen en wijzigen. Locaties worden niet hard verwijderd;
roombeheer gebruikt een actieve-locatiekeuzelijst en locatie-updates behouden alle
roomrelaties en reserveringssnapshots.

## Alternatieven

- Reservations hard verwijderen bij annulering.
- Een gebruikte ruimte fysiek verwijderen.
- Deactiveren inclusief automatische annulering.
- Historie altijd met actuele ruimtegegevens tonen.

## Motivatie

De oorspronkelijke reservering en haar context moeten controleerbaar blijven; de
precieze historische snapshotregel blijft een open productbesluit.

## Consequenties

Data-model en integratietests bewaken statusbehoud, foreign-key-relaties en de
blokkade op deactivering.
