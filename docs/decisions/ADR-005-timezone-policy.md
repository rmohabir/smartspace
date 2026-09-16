# ADR-005: Europe/Amsterdam en zomertijd

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

Gebruikers plannen in lokale BIDN-tijd. Tijdens de overgang naar zomer- of wintertijd
bestaan sommige lokale tijden niet of komen ze dubbel voor.

## Besluit

Gebruik Europe/Amsterdam voor invoer en weergave. Weiger niet-bestaande lokale tijden
en vraag bij dubbel voorkomende tijden een expliciete keuze. Contracten dragen een
expliciete offset of instant.

## Alternatieven

- Lokale tijden automatisch corrigeren.
- Bij dubbele tijden automatisch de eerste variant kiezen.
- Alleen UTC aan de gebruiker tonen.

## Motivatie

Stille verschuiving van een reservering is onaanvaardbaar voor gebruikersvertrouwen.

## Consequenties

Unit- en browsertests moeten beide DST-randen aantonen. De uiteindelijke UX-copy moet
Nederlandstalig en begrijpelijk zijn.
