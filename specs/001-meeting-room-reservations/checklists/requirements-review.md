# Requirements Review Checklist: SmartSpace release 1

**Purpose**: Inhoudelijke review van volledigheid en consistentie van requirements
voor autorisatie, overlap, tijdzones, historie, annulering, ruimtebeheer,
foutgedrag en grensgevallen.
**Created**: 2026-09-16
**Feature**: [spec.md](../spec.md) en [plan.md](../plan.md)

**Note**: Dit is een custom requirements-quality checklist. De beoordeling gaat
alleen over wat in de requirements en het plan is beschreven; er is geen claim dat
code, database of browserflows zijn getest.
**Review Ownership**: Reviewer-owned. Markeer `[x]` alleen wanneer het requirement
zelf compleet, duidelijk, consistent en toetsbaar is.
**Marker Semantics**: `[x]` betekent requirementskwaliteit voldoende; het betekent
niet dat implementatie of tests zijn uitgevoerd.

## Autorisatie en eigenaarschap

- [x] CHK001 Is vastgelegd dat de eigenaar server-side wordt bepaald en niet door een vrije gebruikersinvoer? [Completeness, Spec FR-007]
  - Beoordeling: ja; FR-007 en de medewerker-/reserveringsscenario's maken dit expliciet.
- [x] CHK002 Is de bevoegdheidsgrens tussen medewerker en beheerder consistent beschreven? [Consistency, Spec FR-016]
  - Beoordeling: ja; de beheerder beheert alleen ruimtes en mag geen reserveringen van anderen beheren.
- [x] CHK003 Is ongeautoriseerde inzage onderscheiden van ongeautoriseerde wijziging en annulering? [Coverage, Spec US3/US4]
  - Beoordeling: ja; de scenario's en FR-013/FR-016 noemen inzage, wijzigen en annuleren.
- [x] CHK004 Is het voorkomen van persoonsgegevenslekken onderdeel van het autorisatievereiste? [Security, Spec FR-013]
  - Beoordeling: ja; reserverings-ID's en beschikbaarheid mogen geen eigenaargegevens onthullen.
- [ ] CHK005 Is het beleid voor beheer bij afwezigheid van de eigenaar of tijdens een lopende reservering definitief en ondubbelzinnig? [Gap, Spec Open Product Decisions]
  - Bevinding: nee; dit blijft een open productbesluit en mag niet door implementatie worden ingevuld.

## Overlap en gelijktijdige aanvragen

- [x] CHK006 Is de overlapregel voor actieve reserveringen expliciet en toetsbaar? [Clarity, Spec FR-005]
  - Beoordeling: ja; actieve overlappende reserveringen voor dezelfde ruimte zijn verboden.
- [x] CHK007 Is de grens voor aansluitende tijdvakken expliciet? [Clarity, Spec FR-006]
  - Beoordeling: ja; het eindtijdstip is exclusief en aansluitende intervallen zijn toegestaan.
- [x] CHK008 Zijn gedeeltelijke overlap, insluiting, identiek tijdvak en aansluitend tijdvak afgedekt? [Coverage, Spec US1/US2]
  - Beoordeling: ja; de scenario's en edge cases noemen deze grensgevallen.
- [x] CHK009 Is gelijktijdige aanvraag als afzonderlijk risico beschreven, inclusief de invariant dat hoogstens één aanvraag slaagt? [Coverage, Spec SC-002]
  - Beoordeling: ja; zowel spec als plan noemen concurrente paren en geen overlappende actieve rijen.
- [ ] CHK010 Is het gewenste gedrag bij tijdelijke databasebezetting, onzekere commituitkomst en retry-grenzen als requirement vastgelegd? [Gap, Plan Test Strategy]
  - Bevinding: gedeeltelijk; SQL Server-concurrency is benoemd, maar lock-timeout, retry en onbekende commituitkomst zijn niet als release-1 requirement vastgelegd.

## Tijdzones en lokale tijden

- [x] CHK011 Is Europe/Amsterdam als invoer- en weergavetijdzone vastgelegd? [Clarity, Spec FR-017]
  - Beoordeling: ja; de verduidelijking en FR-017 bevestigen dit.
- [x] CHK012 Is gedrag voor niet-bestaande zomertijdtijden expliciet? [Edge Case, Spec FR-017]
  - Beoordeling: ja; zulke tijden worden geweigerd.
- [x] CHK013 Is gedrag voor dubbel voorkomende wintertijdtijden expliciet? [Edge Case, Spec FR-017]
  - Beoordeling: ja; de medewerker moet expliciet kiezen.
- [ ] CHK014 Is de representatie van een expliciete keuze bij een dubbel lokaal tijdstip voor gebruikers en contract volledig gedefinieerd? [Clarity, Spec FR-017 / Plan OpenAPI]
  - Bevinding: nee; de uitkomst is duidelijk, maar het onderscheidende invoercriterium of contractveld is nog niet beschreven.
- [x] CHK015 Is stille interpretatie door de server- of lokale tijdzone uitgesloten? [Consistency, Spec Edge Cases]
  - Beoordeling: ja; de spec verbiedt stille tijdverschuiving en het plan noemt expliciete offset/instant.

## Historie en annulering

- [x] CHK016 Is annulering beschreven als behoud van historie en vrijgeven van het tijdvak? [Completeness, Spec FR-010/FR-011]
  - Beoordeling: ja; statusbehoud en beschikbaarheid zijn beide opgenomen.
- [ ] CHK017 Is vastgelegd of een herhaalde annulering idempotent succesvol is of een conflict oplevert? [Ambiguity, Spec Labvoorstel]
  - Bevinding: nee; dit is nu expliciet als labvoorstel opgenomen, maar nog geen bevestigd productbesluit.
- [x] CHK018 Is de grens vastgelegd dat reserveringen niet hard worden verwijderd? [Clarity, Spec FR-019]
  - Beoordeling: ja; ook de plan- en datamodelartefacts herhalen dit.
- [ ] CHK019 Is duidelijk welke ruimtegegevens historische reserveringen tonen na een latere ruimtewijziging? [Gap, Spec Open Product Decisions]
  - Bevinding: nee; snapshot bij boeking versus actuele gegevens blijft open.
- [ ] CHK020 Is bewaartermijn en privacybeleid voor historische reserveringen vastgesteld? [Gap, Spec Open Product Decisions]
  - Bevinding: nee; raadpleegbaarheid is genoemd, maar duur en precieze privacyregels ontbreken.
- [x] CHK021 Is de betekenis van "afgelopen" onderscheiden van de status "Cancelled"? [Clarity, Spec Key Entities/FR-012]
  - Beoordeling: ja; afgelopen is een weergavecategorie, geannuleerd een reserveringsstatus.

## Ruimtebeheer en levenscyclus

- [x] CHK022 Zijn toevoegen, wijzigen en deactiveren als afzonderlijke beheeractiviteiten beschreven? [Completeness, Spec US4/FR-014]
  - Beoordeling: ja; alle drie hebben scenario's en requirements.
- [x] CHK023 Zijn naam, positieve capaciteit en locatie als invoerregels benoemd? [Clarity, Spec US4]
  - Beoordeling: ja; ook de foutscenario's benoemen lege naam, niet-positieve capaciteit en ontbrekende locatie.
- [x] CHK024 Is duidelijk dat inactieve ruimtes niet nieuw boekbaar zijn maar historie behouden? [Consistency, Spec FR-015/FR-019]
  - Beoordeling: ja.
- [x] CHK025 Is stilzwijgende annulering bij deactiveren uitgesloten? [Edge Case, Spec US4]
  - Beoordeling: ja; deactivering wordt geweigerd bij lopende of toekomstige actieve reserveringen.
- [ ] CHK026 Is de grens van "lopende" reservering ten opzichte van het actuele tijdstip gedefinieerd? [Ambiguity, Spec Open Product Decisions]
  - Bevinding: nee; dit is toegevoegd als expliciet open besluit, inclusief klokgrens/marge.
- [ ] CHK027 Is heractiveren van een gedeactiveerde ruimte binnen scope of buiten scope verklaard? [Gap, Plan/TechStack]
  - Bevinding: niet eenduidig; TechStack noemt heractiveren als voorstel, maar de spec beschrijft alleen deactiveren.

## Foutgedrag en grensgevallen

- [x] CHK028 Zijn ongeldige tijdvolgorde en niet-begonnen tijdvakken als afwijsbare gevallen beschreven? [Coverage, Spec Edge Cases/FR-003/FR-004]
  - Beoordeling: ja; de toekomstige start blijft herkenbaar als labvoorstel.
- [x] CHK029 Is het behoud van de oorspronkelijke reservering bij een afgewezen wijziging expliciet? [Clarity, Spec FR-009/US3]
  - Beoordeling: ja.
- [x] CHK030 Zijn conflict, stale-versie, ongeautoriseerde toegang en tijdelijke beschikbaarheidsfout onderscheiden? [Completeness, Plan/API]
  - Beoordeling: grotendeels; conflict, autorisatie en tijdelijke fout zijn beschreven. De exacte stale-versie-eis staat in het plan/OpenAPI, niet volledig als genummerde spec-FR.
- [ ] CHK031 Is voor iedere foutcategorie een stabiele gebruikersbetekenis en grens tussen retry, conflict en opnieuw laden vastgelegd? [Clarity, Spec FR-018 / Plan]
  - Bevinding: gedeeltelijk; begrijpelijke meldingen zijn vereist, maar retry/reloadbeleid en alle foutcodes zijn nog niet volledig als productrequirement vastgelegd.
- [x] CHK032 Zijn lege resultaten onderscheiden van technische fouten? [Clarity, Spec US1]
  - Beoordeling: ja.
- [x] CHK033 Zijn scopegrenzen voor werkplekken, notificaties, externe integraties en rapportages expliciet? [Completeness, Spec Out of Scope]
  - Beoordeling: ja.

## Plan/spec-consistentie

- [x] CHK034 Komt SQL Server als persistente bron overeen tussen constitution, plan en ADR-002? [Consistency, Plan Constitution Check]
  - Beoordeling: ja; SQLite is alleen als eerder prototype-alternatief beschreven.
- [ ] CHK035 Is de React/Next.js/controller-Web-API-stack consistent met de huidige constitutionele stackkeuze? [Conflict, Plan Constitution Check]
  - Bevinding: nee; het plan markeert dit terecht CONDITIONAL en ADR-001 als Voorgesteld.
- [x] CHK036 Is het onderscheid tussen requirementsreview en uitvoering/implementatie expliciet? [Scope, Plan/Quickstart]
  - Beoordeling: ja; de artefacts claimen geen code- of producttest.
- [x] CHK037 Is het OpenAPI-contract inhoudelijk consistent met create versus update van ruimtes? [Consistency, OpenAPI]
  - Beoordeling: ja na review; `RoomCreateRequest` vereist geen `version`, update wel.
- [ ] CHK038 Zijn alle voorstellen en open besluiten traceerbaar naar een eigenaar en beslismoment? [Traceability, Spec/Plan]
  - Bevinding: nee; verantwoordelijke BIDN-rollen en besluitmomenten zijn niet ingevuld.

## Notes

- Totaal beoordeelde items: 38.
- `[x]` betekent alleen dat de requirementskwaliteit voor dat item voldoende is;
  het betekent niet dat code, SQL Server, API of browser is uitgevoerd of getest.
- Open punten blijven bewust ongecheckt: beheer bij afwezigheid/lopende boeking,
  historische room-snapshots, bewaartermijn/privacy, definitie van "lopende",
  heractiveren, fout-retrybeleid en formele stackgoedkeuring.
- De bestaande `checklists/requirements.md` is de generieke Spec Kit-checklist en
  blijft afzonderlijk bestaan.
- Volgende stap voor requirements: productbesluiten sluiten en de constitutionele
  stackafwijking goedkeuren vóór `/speckit-tasks` of implementatie.
