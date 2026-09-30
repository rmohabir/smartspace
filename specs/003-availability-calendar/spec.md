# Feature Specification: SmartSpace beschikbaarheidskalender

**Feature Branch**: `003-availability-calendar`

**Created**: 2026-09-30

**Status**: Draft

**Input**: User description: "Een agenda-/kalenderoverzicht onder het menu Beschikbaarheid
dat per week toont welke vergaderruimtes op welke tijdvakken en locaties zijn geboekt en
welke vrij zijn. Klikken op een vrij tijdvak start een reservering, vergelijkbaar met
Outlook maar dan voor ruimtes. De gebruiker kan per week vooruit en achteruit bladeren."

## Samenvatting

Deze feature voegt een wekelijkse kalenderweergave toe aan de bestaande pagina
**Beschikbaarheid** (`/beschikbaarheid`). De medewerker ziet in één rooster welke
vergaderruimtes per tijdvak vrij of bezet zijn, gefilterd op locatie, en kan per week
vooruit- en achteruitbladeren. Een klik op een vrij tijdvak opent het bestaande
reserveerformulier met de gekozen ruimte en tijd voorgevuld. De kalender is een nieuwe
visualisatie boven op bestaande release-1 gegevens (ruimtes, locaties, reserveringen) en
introduceert geen nieuwe boekingsregels.

De feature valt binnen de release-1 grens uit de constitution: geen terugkerende
boekingen, notificaties, deelnemerslijsten, check-in/-out of externe agenda-integratie.
"Vergelijkbaar met Outlook" beschrijft uitsluitend de visuele rasterinteractie, niet een
integratie met Outlook, Teams of Microsoft Graph.

## Clarifications

De volgende punten zijn nog niet door BIDN bevestigd en worden als **labvoorstel**
behandeld (constitution Principle X). Ze zijn met een redelijke standaardkeuze ingevuld
zodat de feature implementeerbaar is; een `[NEEDS CLARIFICATION]`-markering blijft staan
waar bevestiging vereist is voordat de betrokken user story wordt geïmplementeerd.

- **Labvoorstel — Weergavebereik**: de kalender toont standaard een werkweek van maandag
  tot en met vrijdag. De gebruiker kan geen losse dagen verbergen in release 1.
  `[NEEDS CLARIFICATION: werkweek (ma–vr) of volledige week (ma–zo)?]`
- **Labvoorstel — Tijdraster**: de verticale as toont dagdelen in blokken van 30 minuten
  binnen een standaard zichtbaar venster van 07:00–19:00 Europe/Amsterdam, met verticaal
  scrollen naar eerdere of latere uren op dezelfde dag.
  `[NEEDS CLARIFICATION: rasterinterval 15, 30 of 60 minuten? Zichtbaar dagvenster?]`
- **Labvoorstel — Privacy van bezette tijdvakken**: een bezet tijdvak van een andere
  medewerker toont uitsluitend "Bezet" zonder naam, onderwerp of eigenaargegevens. De
  eigen reserveringen van de ingelogde medewerker worden herkenbaar gemarkeerd. Deze
  keuze volgt release-1 FR-013 en constitution Principle III/VIII en is geen voorstel maar
  een afgeleide eis.
- **Labvoorstel — Klik-op-vrij-tijdvak**: klikken op een vrij tijdvak opent het bestaande
  reserveerformulier met ruimte, begintijd en eindtijd voorgevuld; het legt niet direct
  een reservering vast. De definitieve boeking blijft onder alle bestaande
  beschikbaarheids-, tijd- en overlapregels vallen.
- **Labvoorstel — Standaard boekingsduur**: een klik op één rastercel stelt standaard een
  tijdvak van 30 minuten voor, dat de gebruiker vóór bevestiging kan aanpassen.
  `[NEEDS CLARIFICATION: standaardduur bij één klik en of slepen over meerdere cellen nodig is voor release 1]`

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Wekelijkse beschikbaarheid overzien (Priority: P1)

Als medewerker wil ik in een wekelijks kalenderrooster zien welke vergaderruimtes op welke
tijdvakken en locaties vrij of bezet zijn, zodat ik in één oogopslag een geschikt vrij
moment vind zonder losse zoekopdrachten uit te voeren.

**Why this priority**: Dit overzicht is de kern van de gevraagde waarde: een visueel,
Outlook-achtig beeld van bezetting per week. Zonder dit rooster bestaat de feature niet.

**Independent Test**: Geef meerdere actieve ruimtes op verschillende locaties met een mix
van actieve reserveringen in de huidige week. Open de kalender onder Beschikbaarheid en
controleer dat elk tijdvak correct als vrij of bezet wordt getoond en dat de weergave per
ruimte en dag klopt met de onderliggende reserveringen.

**Acceptance Scenarios**:

1. **Given** actieve ruimtes met actieve reserveringen in de huidige week, **When** de
   medewerker de kalender opent, **Then** toont het rooster per ruimte en tijdvak duidelijk
   onderscheid tussen vrije en bezette blokken voor de zichtbare week.
2. **Given** een ruimte met een reservering van 10:00–11:00, **When** de medewerker die dag
   bekijkt, **Then** is 10:00–11:00 als bezet gemarkeerd en zijn direct aansluitende
   tijdvakken (bijvoorbeeld 09:30–10:00 en 11:00–11:30) als vrij zichtbaar.
3. **Given** een geannuleerde reservering in de zichtbare week, **When** de medewerker de
   kalender bekijkt, **Then** blokkeert deze geen tijdvak en wordt het betrokken blok als
   vrij getoond.
4. **Given** een gedeactiveerde ruimte of een ruimte op een inactieve locatie, **When** de
   medewerker de kalender bekijkt, **Then** verschijnt die ruimte niet als boekbare rij in
   het rooster.
5. **Given** er zijn geen ruimtes die aan het actieve filter voldoen, **When** de kalender
   laadt, **Then** ziet de medewerker een begrijpelijke lege-staatmelding en geen
   technische fout.

---

### User Story 2 - Vrij tijdvak aanklikken en reserveren (Priority: P1)

Als medewerker wil ik een vrij tijdvak in de kalender aanklikken om direct een reservering
te starten voor die ruimte en tijd, zodat ik snel en zonder overtypen kan boeken.

**Why this priority**: De directe klik-naar-boeking is het tweede kernonderdeel van de
gevraagde Outlook-achtige interactie en verbindt het overzicht met de bestaande
boekingsflow.

**Independent Test**: Klik op een vrij tijdvak in de kalender en controleer dat het
reserveerformulier opent met de juiste ruimte, begintijd en eindtijd voorgevuld, en dat het
bevestigen één reservering vastlegt die daarna als bezet in de kalender verschijnt.

**Acceptance Scenarios**:

1. **Given** een vrij tijdvak in de kalender, **When** de medewerker erop klikt, **Then**
   opent het bestaande reserveerformulier met de betrokken ruimte, begintijd en eindtijd
   voorgevuld.
2. **Given** het voorgevulde formulier, **When** de medewerker bevestigt, **Then** wordt
   één actieve reservering op naam van de medewerker vastgelegd en verschijnt het tijdvak
   bij een volgende laadbeurt als bezet.
3. **Given** een bezet tijdvak, **When** de medewerker erop klikt, **Then** start er geen
   reservering en biedt de kalender geen boekactie aan voor dat blok.
4. **Given** een ander persoon boekt hetzelfde tijdvak vlak vóór bevestiging, **When** de
   medewerker het formulier bevestigt, **Then** wordt de aanvraag met een begrijpelijke
   conflictmelding afgewezen en ontstaan geen twee overlappende actieve reserveringen.
5. **Given** een gekozen tijdvak ligt in het verleden, **When** de medewerker probeert te
   boeken, **Then** wordt de aanvraag afgewezen volgens de bestaande toekomstregel.

---

### User Story 3 - Door weken bladeren (Priority: P1)

Als medewerker wil ik per week vooruit- en achteruitbladeren en snel naar de huidige week
terugkeren, zodat ik beschikbaarheid in komende of eerdere weken kan bekijken.

**Why this priority**: Navigatie per week is expliciet gevraagd en maakt de kalender
bruikbaar buiten de huidige week; zonder navigatie is het overzicht te beperkt.

**Independent Test**: Blader vanaf de huidige week één week vooruit en één week achteruit en
controleer dat de getoonde datums, dagkoppen en bezetting per week correct wijzigen en dat
een "deze week"-actie terugkeert naar de huidige week.

**Acceptance Scenarios**:

1. **Given** de kalender toont de huidige week, **When** de medewerker "volgende week"
   kiest, **Then** verschuift het rooster één week vooruit met bijgewerkte datums en
   bezetting.
2. **Given** de kalender toont een andere week, **When** de medewerker "vorige week" kiest,
   **Then** verschuift het rooster één week achteruit met bijgewerkte datums en bezetting.
3. **Given** de kalender toont een andere week dan de huidige, **When** de medewerker "deze
   week" kiest, **Then** keert het rooster terug naar de week die de huidige datum bevat.
4. **Given** de medewerker bladert over een overgang naar of van zomertijd, **When** de
   betrokken week wordt getoond, **Then** blijven de dag- en tijdlabels correct in
   Europe/Amsterdam en verschuift de bezetting niet stilzwijgend.

---

### User Story 4 - Filteren op locatie en capaciteit (Priority: P2)

Als medewerker wil ik de kalender filteren op locatie en minimumcapaciteit, zodat ik alleen
relevante ruimtes in het rooster zie.

**Why this priority**: Filtering houdt het rooster overzichtelijk bij meerdere locaties en
sluit aan op de bestaande beschikbaarheidsfilters, maar is ondergeschikt aan het rooster
zelf.

**Independent Test**: Kies een locatie en een minimumcapaciteit en controleer dat alleen
ruimtes die aan beide voldoen als rijen in de kalender verschijnen en dat het legen van de
filters weer alle actieve ruimtes toont.

**Acceptance Scenarios**:

1. **Given** ruimtes op meerdere locaties, **When** de medewerker één locatie kiest,
   **Then** toont de kalender alleen actieve ruimtes van die locatie.
2. **Given** ruimtes met verschillende capaciteiten, **When** de medewerker een
   minimumcapaciteit kiest, **Then** verschijnen alleen ruimtes met minstens die capaciteit.
3. **Given** een actief filter zonder passende ruimtes, **When** de kalender laadt, **Then**
   toont de weergave een begrijpelijke lege-staatmelding.

---

### Edge Cases

- Een reservering die de grens van het zichtbare dagvenster overschrijdt (bijvoorbeeld
  06:30–07:30) wordt correct als bezet getoond voor het deel dat binnen het venster valt,
  zonder dat er beschikbaarheid verloren gaat buiten het venster.
- Een reservering die twee kalenderdagen overspant wordt op elke betrokken dag als bezet
  getoond voor het juiste deel.
- Een reservering die precies op een tijdvakgrens begint of eindigt laat het aansluitende
  tijdvak vrij, omdat het eindtijdstip exclusief is.
- Bij een overgang naar zomertijd bestaan bepaalde lokale tijden niet; die tijdvakken
  worden niet als boekbaar aangeboden. Bij een overgang van zomertijd komt een lokaal
  tijdstip dubbel voor; de bestaande expliciete-offsetkeuze uit release 1 blijft van
  toepassing bij het boeken.
- Wanneer bezetting tijdelijk niet kan worden opgehaald, toont de kalender een begrijpelijke
  foutmelding met opnieuw-proberen en presenteert geen deels geladen week als volledig.
- De kalender toont nooit naam, onderwerp of eigenaargegevens van reserveringen van andere
  medewerkers; een bezet blok blijft anoniem voor niet-eigenaren.
- Een medewerker die niet is ingelogd kan de kalender niet gebruiken om te boeken; de
  boekactie vereist een geldige identiteit volgens de bestaande autorisatie.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Het systeem MUST onder het menu Beschikbaarheid een wekelijkse
  kalenderweergave aanbieden die per actieve vergaderruimte de vrije en bezette tijdvakken
  van de zichtbare week toont.
- **FR-002**: Het systeem MUST uitsluitend actieve ruimtes op actieve locaties als
  boekbare rijen in de kalender opnemen en gedeactiveerde ruimtes of ruimtes op inactieve
  locaties uitsluiten.
- **FR-003**: Het systeem MUST bezette tijdvakken bepalen op basis van actieve
  reserveringen en geannuleerde reserveringen niet als bezetting tonen.
- **FR-004**: Het systeem MUST aansluitende tijdvakken correct als vrij tonen op basis van
  het exclusieve eindtijdstip, zonder buffer.
- **FR-005**: Het systeem MUST de medewerker per week vooruit en achteruit laten bladeren en
  een actie bieden om naar de week met de huidige datum terug te keren.
- **FR-006**: Het systeem MUST de zichtbare week, dagkoppen, datums en tijdvakken tonen in
  Europe/Amsterdam en niet-bestaande lokale tijden niet als boekbaar tijdvak aanbieden.
- **FR-007**: Het systeem MUST een klik op een vrij tijdvak omzetten naar het openen van de
  bestaande reserveerflow met de betrokken ruimte, begintijd en eindtijd voorgevuld, zonder
  direct een reservering vast te leggen.
- **FR-008**: Het systeem MUST voor een bezet tijdvak geen boekactie aanbieden en de klik
  niet als boekingsstart interpreteren.
- **FR-009**: Het systeem MUST bij het bevestigen van een via de kalender gestarte
  reservering alle bestaande release-1 regels toepassen: geldige toekomstige tijd,
  eindtijd na begintijd, geen overlap met een actieve reservering en eigenaar afgeleid van
  de ingelogde identiteit.
- **FR-010**: Het systeem MUST voorkomen dat de kalender naam, onderwerp of eigenaargegevens
  van reserveringen van andere medewerkers toont; bezette blokken van anderen blijven
  anoniem.
- **FR-011**: Het systeem MUST de eigen reserveringen van de ingelogde medewerker herkenbaar
  van andermans bezette blokken onderscheiden, zonder daarbij andermans gegevens prijs te
  geven.
- **FR-012**: Het systeem MUST de kalender laten filteren op locatie en minimumcapaciteit,
  consistent met de bestaande beschikbaarheidsfilters, en bij geen resultaat een
  begrijpelijke lege-staatmelding tonen.
- **FR-013**: Het systeem MUST bezettingsgegevens voor de zichtbare week en het actieve
  filter ophalen op een manier die de weergave in één samenhangende laadbeurt kan vullen,
  zonder per ruimte een losse handmatige zoekopdracht te vereisen.
- **FR-014**: Het systeem MUST bij een tijdelijke fout in het ophalen van bezetting een
  begrijpelijke melding met opnieuw-proberen tonen en geen onvolledig geladen week als
  volledig presenteren.
- **FR-015**: Het systeem MUST de kalender volledig met het toetsenbord bedienbaar maken,
  inclusief weeknavigatie, filters en het starten van een boeking, met zichtbare labels en
  focusstaten en met status die niet uitsluitend via kleur wordt gecommuniceerd.
- **FR-016**: Het systeem MUST reserveringen die het zichtbare dagvenster of een dagovergang
  overschrijden correct per dag als bezet tonen voor het betrokken deel.
- **FR-017**: Het systeem MUST de kalender-, navigatie-, filter- en boekscenario's
  traceerbaar houden naar requirements en geautomatiseerde tests.

### Non-Functional Requirements

- **NFR-001**: De kalender MUST een week met een representatieve trainingsdataset laden
  binnen een voor demogebruik acceptabele responstijd zonder de UI te blokkeren.
  `[NEEDS CLARIFICATION: concrete laadtijddoel en maximale dataset-omvang voor de demo]`
- **NFR-002**: De weergave MUST bruikbaar blijven op de gedocumenteerde viewportmaten
  zonder horizontale overflow buiten het kalenderrooster zelf.

### Open Product Decisions and Lab Proposals

De volgende punten zijn herkenbaar als voorstel of besluitpunt en zijn niet stilzwijgend
goedgekeurd (constitution Principle X):

- **Labvoorstel**: standaard werkweekweergave maandag–vrijdag met verticaal venster
  07:00–19:00 en een rasterinterval van 30 minuten.
- **Labvoorstel**: één klik stelt een tijdvak van 30 minuten voor; slepen over meerdere
  cellen is nog niet als release-1 eis bevestigd.
- **Afgeleide eis (geen voorstel)**: bezette blokken van andere medewerkers blijven anoniem,
  volgend uit release-1 FR-013 en constitution Principle III/VIII.
- **Afgeleide eis (geen voorstel)**: alle boekingsregels en tijdzoneregels van release 1
  blijven onverkort gelden; de kalender voegt geen nieuwe boekingsregels toe.
- **Openstaand**: keuze werkweek versus volledige week, exact rasterinterval en zichtbaar
  dagvenster, standaard boekingsduur en noodzaak van slepen, en het concrete laadtijddoel.

### Key Entities *(include if feature involves data)*

- **Kalenderweek**: het afgeleide zichtbare bereik van zeven of vijf opeenvolgende dagen in
  Europe/Amsterdam waarvoor bezetting wordt getoond; heeft een begin- en einddatum en een
  relatie tot de huidige datum. Dit is een weergaveconcept, geen opgeslagen entiteit.
- **Tijdvakcel**: een raster van een ruimte gekruist met een tijdsinterval binnen de week,
  met een afgeleide status vrij of bezet. Weergaveconcept.
- **Bezettingsblok**: een afgeleide, geanonimiseerde weergave van een actieve reservering
  binnen de zichtbare week: ruimte, begintijd en eindtijd, zonder eigenaar- of
  onderwerpgegevens voor niet-eigenaren.
- **Vergaderruimte**, **Locatie** en **Reservering**: bestaande release-1 entiteiten; deze
  feature voegt geen velden of statussen toe en hergebruikt hun bestaande regels.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In een testset van minimaal 20 combinaties van ruimtes en reserveringen komt
  de vrij/bezet-weergave in 100% van de gevallen overeen met de onderliggende actieve
  reserveringen voor de zichtbare week.
- **SC-002**: In een begeleide gebruikerstest van maximaal 2 minuten kan een medewerker via
  de kalender een vrij tijdvak vinden, aanklikken en een geldige reservering bevestigen;
  minimaal 9 van de 10 deelnemers voltooien dit zonder hulp.
- **SC-003**: In 100% van de privacycontroles toont de kalender geen naam, onderwerp of
  eigenaargegevens van reserveringen van andere medewerkers.
- **SC-004**: In een navigatietest levert vooruit-, achteruit- en "deze week"-bladeren in
  100% van de gevallen de juiste week met correcte datums en bezetting op, inclusief een
  week met een zomer-/wintertijdovergang.
- **SC-005**: Alle beschreven scenario's voor weergave, klik-naar-boeking, weeknavigatie,
  filtering, aansluitende tijdvakken, geannuleerde reserveringen en dagoverschrijdende
  reserveringen hebben een geslaagde geautomatiseerde acceptatietest vóór de demo.
- **SC-006**: In een toegankelijkheidscontrole kan een gebruiker de volledige
  kalenderworkflow met het toetsenbord uitvoeren en vindt voor elke fout een begrijpelijke
  tekstuele uitleg.
- **SC-007**: In een conflicttest waarbij hetzelfde tijdvak vlak vóór bevestiging door een
  ander wordt geboekt, ontstaat nooit een tweede overlappende actieve reservering en krijgt
  de medewerker een begrijpelijke conflictmelding.

## Assumptions

- De feature bouwt voort op de bestaande release-1 entiteiten en endpoints voor ruimtes,
  locaties, beschikbaarheid en reserveringen; er worden geen nieuwe boekingsregels
  geïntroduceerd.
- Een betrouwbare inlogcontext bestaat voor het boeken; de kalender kan bezetting tonen,
  maar boeken vereist een geldige identiteit.
- Europe/Amsterdam is de invoer- en weergavetijdzone, consistent met release 1, inclusief
  de afhandeling van niet-bestaande en dubbel voorkomende lokale tijden.
- De trainingsdemo gebruikt synthetische gegevens en vormt geen productieacceptatie.
- "Vergelijkbaar met Outlook" beschrijft uitsluitend de visuele rasterinteractie en niet een
  integratie met Outlook, Teams of Microsoft Graph.
- De bestaande privacyregels blijven leidend: alleen de eigenaar (en een bevoegde beheerder
  volgens beleid) ziet reserveringsdetails; de kalender toont voor anderen enkel anonieme
  bezetting.

## Out of Scope

De volgende uitbreidingen worden uitsluitend als buiten scope geregistreerd en zijn geen
onderdeel van deze feature:

- Integratie met Outlook, Teams, Microsoft Graph of andere externe agenda's.
- Terugkerende of herhaalde boekingen vanuit de kalender.
- Notificaties, herinneringen of uitnodigingen.
- Deelnemerslijsten, vergadertitels of uitgebreide omschrijvingen in de kalender.
- Check-in en check-out.
- Werkplekken of andere resourcetypes dan vergaderruimtes.
- Slepen-en-neerzetten om bestaande reserveringen te verplaatsen of te verlengen, tenzij een
  later besluit dit als release-eis toevoegt.
- Rapportages, bezettingsanalyse of exportfuncties.
- Productieacceptatie, definitieve hosting en overige operationele uitwerking.
