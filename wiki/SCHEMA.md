# SmartSpace Wiki – Schema

Deze wiki documenteert het **SmartSpace applicatiedomein**: het centraal reserveren
van vergaderruimtes voor BIDN. Het is een kennisbank op basis van goedgekeurde
stakeholder- en ontwerpdocumenten, niet de broncode zelf.

## Domeingrens

- In scope: vergaderruimtes, ruimtebeheer, beschikbaarheid, boeken, wijzigen,
  annuleren, eigen reserveringen en historie (Release 1).
- Buiten scope: werkplekken, check-in/check-out, Outlook, Teams, Microsoft Graph,
  rapportages en capaciteitsanalyse.
- Leid geen businessregels af die niet in de brondocumenten, specs of tests staan.
- Onderscheid **oorspronkelijke businessregels** expliciet van **labvoorstellen**.

## Mappenstructuur

- `raw/` — onaanraakbare kopieën van goedgekeurde bronnen. Lezen, nooit bewerken.
- `domains/` — domeinoverzichten.
- `people/` — rollen en betrokkenen.
- `entities/` — concrete entiteiten (ruimtes, reserveringen, gebruikers).
- `concepts/` — regels, principes en begrippen.
- `comparisons/` — vergelijkingen tussen opties of voorstellen.
- `queries/` — gefileerde, herbruikbare antwoorden.
- `summaries/` — syntheses over meerdere pagina's.
- `_archive/` — verouderde inhoud (oorspronkelijk subpad behouden).

## Paginaconventies

- Bestandsnamen: kleine letters, koppeltekens.
- Elke pagina heeft YAML-frontmatter met alle velden hieronder.
- Wikilinks zijn wiki-root-relatief en bevatten de map, bijv.
  `[[concepts/boekingsintegriteit]]`. Geen basename-only links.
- Elke nieuwe/gewijzigde pagina: traceerbare bron, minstens twee zinvolle
  wikilinks, opgenomen in `index.md`, vastgelegd in `log.md`.
- Splits een pagina die groter wordt dan ~200 regels.
- Voeg op pagina's die drie of meer bronnen synthetiseren een
  `^[raw/bronbestand.md]`-marker toe aan elke alinea.

## Frontmatter

```yaml
---
title: Paginatitel
created: YYYY-MM-DD
updated: YYYY-MM-DD
type: domain | person | entity | concept | comparison | query | summary
tags: [reservations]
sources: [raw/projectgoals.md]
confidence: high | medium | low
contested: false
contradictions: []
---
```

## Taxonomie (toegestane tags)

Gebruik uitsluitend deze tags:

- `reservations` — reserveren, wijzigen, annuleren, historie.
- `room-management` — ruimtebeheer en ruimtelevenscyclus.
- `business-rules` — oorspronkelijke, goedgekeurde businessregels.
- `lab-proposal` — voorstellen die nog validatie/goedkeuring nodig hebben.
- `scope` — scope- en out-of-scope-afbakening.
- `governance` — validatie, review en acceptatiemomenten.

## Onzekerheid en tegenstrijdigheden

- `confidence`: `high` bij directe brondekking, anders `medium`/`low`.
- Markeer een pagina `contested: true` bij conflicterende bronnen en noteer de
  tegenstrijdigheid in `contradictions`.
- Los onzekerheid nooit stil op. Menselijke review is vereist voor
  bronselectie, feitelijke juistheid, confidence en tegenstrijdigheden.
