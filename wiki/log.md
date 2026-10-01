# SmartSpace Wiki – Log

Append-only activiteitenlog. Nieuwste bovenaan.

## 2026-09-30 — Ingest: AppFeatures.md en TechStack.md

**Actie:** Bulk-ingest van twee goedgekeurde bronnen.

**Bronnen (raw-kopieën):**

- `raw/appfeatures.md` (kopie van `StakeholderDocuments/AppFeatures.md`)
- `raw/techstack.md` (kopie van `StakeholderDocuments/TechStack.md`)

**Aangemaakte pagina's (3, binnen de paginalimiet):**

- `concepts/businessregels.md` — geconsolideerde regels, eis vs voorstel, met provenance-markers.
- `people/rollen-en-autorisatie.md` — rollen en autorisatiematrix.
- `concepts/techstack-en-architectuur.md` — techstack, architectuur, opslagbeperking, concurrency, tijdzone.

**Gewijzigd:** `index.md`.

**Tags gebruikt:** `business-rules`, `reservations`, `lab-proposal`, `governance`, `room-management`, `scope`.

**Confidence:** high (directe brondekking).

**Onzekerheid/te valideren door BIDN:** buffers, boekingshorizon, openings-/
duurregels, beheerbevoegdheden over andermans boekingen, duurzame opslag,
identiteitskoppeling (Entra ID) en hosting. Deze zijn expliciet als voorstel
gemarkeerd, niet als vaststaand feit.

**Nog niet geïngest (kandidaten volgende ronde):** Minimal API-contract (§8),
datamodel (§5), UI-ontwerp (§10), acceptatiescenario's (§14), open besluiten
(§15). Vraagt aparte ingest wegens paginalimiet.

**Reviewmoment:** Menselijke review vereist op eis-vs-voorstel-classificatie en
feitelijke juistheid voordat deze pagina's als geaccepteerde kennis gelden.

## 2026-09-30 — Review geaccepteerd

**Actie:** Menselijke review afgerond en geaccepteerd.

**Uitkomst:**

1. Bronselectie (`raw/projectgoals.md`) en feitelijke inhoud bevestigd.
2. Onderscheid oorspronkelijke businessregel (geen overlappende boekingen)
   versus labvoorstellen bevestigd.

De initiële pagina's gelden vanaf nu als geaccepteerde kennis.

## 2026-09-30 — Wiki geïnitialiseerd

**Actie:** Start van de wiki (initialisatie).

**Domein:** SmartSpace applicatiedomein — centraal reserveren van vergaderruimtes
voor BIDN (Release 1).

**Eerste trainingsvraag:** "Documentatie van de SmartSpace-app" — wat is het
domein, wat valt binnen Release 1, en welke oorspronkelijke businessregel geldt.

**Scope:** Klein en testbaar. Basisstructuur plus drie samenhangende pagina's.

**Goedgekeurde bronnen:**

- `raw/projectgoals.md` (kopie van `StakeholderDocuments/ProjectGoals.md`)

**Taxonomietags gebruikt:** `reservations`, `room-management`, `business-rules`,
`scope`.

**Gewijzigde/aangemaakte bestanden:**

- `SCHEMA.md` (nieuw)
- `index.md` (nieuw)
- `log.md` (nieuw)
- `raw/projectgoals.md` (nieuw)
- `domains/smartspace-vergaderruimte-reserveringen.md` (nieuw)
- `concepts/boekingsintegriteit.md` (nieuw)
- `concepts/scope-release-1.md` (nieuw)

**Bekende onzekerheid:** De brondocumenten onderscheiden oorspronkelijke
businessregels van labvoorstellen (boekingshorizon, buffers, beheerrechten,
identiteit, opslag, transactietechniek). Die voorstellen zijn nog niet
goedgekeurd en zijn bewust niet als vaste feiten vastgelegd.

**Reviewmoment:** Menselijke review vereist op bronselectie, feitelijke
juistheid, confidence en het onderscheid regel-versus-voorstel voordat deze
pagina's als geaccepteerde kennis gelden.
