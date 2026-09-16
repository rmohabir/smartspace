# Feature Specification: SmartSpace release 1 vergaderruimtes

**Feature Branch**: `001-meeting-room-reservations`

**Created**: 2026-09-16

**Status**: Draft

**Input**: User description: "Specificeer SmartSpace release 1 voor het reserveren van vergaderruimtes binnen BIDN"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Beschikbare vergaderruimte vinden (Priority: P1)

Als medewerker wil ik vergaderruimtes zoeken voor een gekozen tijdvak, zodat ik snel
kan zien welke ruimte geschikt en vrij is voor mijn overleg.

**Why this priority**: Beschikbaarheid is de noodzakelijke ingang voor betrouwbaar
reserveren en voorkomt onnodige boekingspogingen.

**Independent Test**: Geef een medewerker meerdere ruimtes met verschillende locaties,
capaciteiten en reserveringen. De medewerker kan een tijdvak en filters kiezen en
krijgt uitsluitend passende vrije vergaderruimtes met begrijpelijke resultaten.

**Acceptance Scenarios**:

1. **Given** actieve ruimtes met verschillende locaties en capaciteiten, **When** de
   medewerker een geldig toekomstig tijdvak en een minimumcapaciteit kiest, **Then**
   worden alleen ruimtes getoond die in dat volledige tijdvak vrij zijn en aan de
   filters voldoen.
2. **Given** een ruimte met een actieve reservering die gedeeltelijk overlapt,
   **When** de medewerker hetzelfde tijdvak zoekt, **Then** wordt die ruimte niet als
   beschikbaar getoond.
3. **Given** een ruimte met een reservering die precies eindigt wanneer het gekozen
   tijdvak begint, **When** de medewerker zoekt, **Then** blijft de ruimte beschikbaar
   voor het aansluitende tijdvak.
4. **Given** geen passende ruimtes, **When** de medewerker de zoekopdracht uitvoert,
   **Then** ziet de medewerker een duidelijke melding dat er geen resultaten zijn,
   zonder dat dit als technische fout wordt gepresenteerd.

### User Story 2 - Vergaderruimte boeken (Priority: P1)

Als medewerker wil ik een beschikbare vergaderruimte voor mezelf reserveren, zodat
ik mijn overleg kan plannen.

**Why this priority**: De kernwaarde van SmartSpace is dat medewerkers een ruimte
betrouwbaar kunnen vastleggen.

**Independent Test**: Laat een medewerker een geldige vrije ruimte en tijd kiezen,
de reservering bevestigen en daarna de reservering terugvinden in het eigen overzicht.

**Acceptance Scenarios**:

1. **Given** een actieve ruimte is vrij voor een geldig toekomstig tijdvak, **When**
   de medewerker de reservering bevestigt, **Then** wordt één actieve reservering op
   naam van die medewerker vastgelegd en is deze zichtbaar in het eigen overzicht.
2. **Given** twee medewerkers dienen gelijktijdig een overlappende reservering voor
   dezelfde ruimte in, **When** beide aanvragen worden verwerkt, **Then** slaagt
   hoogstens één aanvraag en krijgt de andere medewerker een begrijpelijke
   conflictmelding; er bestaan nooit twee overlappende actieve reserveringen.
3. **Given** een ruimte is niet beschikbaar voor het gevraagde tijdvak, **When** de
   medewerker probeert te boeken, **Then** wordt de reservering afgewezen en wordt
   geen nieuwe actieve reservering aangemaakt.
4. **Given** een aanvraag voor een tijdvak sluit direct aan op een bestaande actieve
   reservering, **When** de medewerker boekt, **Then** wordt de aanvraag toegestaan
   zolang geen bevestigd beleid een buffer vereist.

### User Story 3 - Eigen reserveringen en historie beheren (Priority: P1)

Als medewerker wil ik mijn komende, afgelopen en geannuleerde reserveringen bekijken,
zodat ik mijn afspraken kan controleren en beheren.

**Why this priority**: Eigen overzicht en historie zijn nodig om reserveringen te
vertrouwen, terug te vinden en aantoonbaar te beheren.

**Independent Test**: Maak voor één medewerker komende, afgelopen en geannuleerde
reserveringen aan. Controleer dat alle drie categorieën zichtbaar zijn en dat een
andere medewerker deze persoonlijke historie niet kan inzien.

**Acceptance Scenarios**:

1. **Given** een medewerker heeft komende, afgelopen en geannuleerde reserveringen,
   **When** de medewerker het eigen overzicht opent, **Then** kan de medewerker de
   drie categorieën afzonderlijk bekijken met ruimte, locatie, datum en tijdvak.
2. **Given** een medewerker opent een reservering van een andere medewerker, **When**
   die medewerker het reserveringskenmerk gebruikt, **Then** krijgt die medewerker
   geen inzage of wijzigingsmogelijkheid.
3. **Given** een actieve reservering van de eigenaar ligt in de toekomst, **When** de
   eigenaar deze wil wijzigen, **Then** kan de eigenaar ruimte en/of tijd aanpassen,
   onder dezelfde beschikbaarheids- en tijdregels als bij boeken.
4. **Given** een actieve reservering van de eigenaar ligt in de toekomst, **When** de
   eigenaar deze annuleert, **Then** blijft de historie zichtbaar, verandert de
   reservering naar geannuleerd en komt het tijdvak opnieuw vrij.
5. **Given** een wijziging overlapt met een andere actieve reservering, **When** de
   eigenaar de wijziging bevestigt, **Then** wordt de wijziging afgewezen en blijft
   de oorspronkelijke reservering ongewijzigd.

### User Story 4 - Ruimtes beheren (Priority: P2)

Als beheerder wil ik vergaderruimtes toevoegen, aanpassen en deactiveren, zodat de
ruimtecatalogus betrouwbaar blijft.

**Why this priority**: Goed ruimtebeheer maakt beschikbaarheidsinformatie bruikbaar,
maar is minder frequent dan het dagelijkse zoeken en boeken.

**Independent Test**: Gebruik een beheerdersaccount om een ruimte toe te voegen, de
naam, capaciteit of locatie aan te passen en een ruimte zonder lopende of toekomstige
reserveringen te deactiveren. Controleer dat een medewerker alleen actieve ruimtes
kan vinden.

**Acceptance Scenarios**:

1. **Given** een bevoegde beheerder voert een naam, positieve capaciteit en locatie
   in, **When** de beheerder de ruimte opslaat, **Then** verschijnt de ruimte in de
   catalogus en kan zij worden gevonden bij beschikbaarheid.
2. **Given** een beheerder probeert een ruimte ongeldig te wijzigen, **When** de
   beheerder een lege naam, niet-positieve capaciteit of ontbrekende locatie indient,
   **Then** wordt de wijziging afgewezen met veldgerichte uitleg.
3. **Given** een ruimte heeft geen lopende of toekomstige actieve reserveringen,
   **When** de beheerder haar deactiveert, **Then** kan zij niet meer nieuw worden
   gereserveerd en blijft bestaande historie raadpleegbaar volgens de privacyregels.
4. **Given** een ruimte heeft een lopende of toekomstige actieve reservering, **When**
   de beheerder haar probeert te deactiveren, **Then** wordt de actie geweigerd en
   blijven bestaande reserveringen intact.
5. **Given** een medewerker zonder beheerdersrechten probeert ruimtebeheer te
   gebruiken, **When** de medewerker een beheeractie uitvoert, **Then** wordt de
   actie geweigerd en worden geen gegevens gewijzigd.

### Edge Cases

- Een eindtijd die gelijk is aan of vóór de begintijd is ongeldig; de aanvraag wordt
  niet opgeslagen.
- Een nieuw of gewijzigd tijdvak dat al is begonnen of verstreken wordt afgewezen;
  dit is een labvoorstel dat vóór productgebruik moet worden bevestigd.
- Een zoekopdracht met een ontbrekend, onjuist of dubbelzinnig lokaal tijdstip wordt
  niet stilzwijgend geïnterpreteerd; de medewerker krijgt een keuze of foutmelding.
- Een geannuleerde reservering blokkeert geen nieuw aansluitend of overlappend
  tijdvak.
- Een wijziging met een verouderde weergave mag geen recente wijziging overschrijven;
  de medewerker moet opnieuw kunnen laden.
- Een onbekende of niet-toegankelijke reservering geeft geen persoonsgegevens prijs.
- Een beheerder mag een ruimte niet deactiveren door lopende of toekomstige
  reserveringen stilzwijgend te annuleren.
- Foutmeldingen blijven begrijpelijk wanneer beschikbaarheid tijdelijk niet kan
  worden opgehaald of opgeslagen.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Het systeem MUST medewerkers actieve vergaderruimtes laten bekijken met
  naam, capaciteit en locatie.
- **FR-002**: Het systeem MUST beschikbaarheid bepalen voor het volledige gekozen
  begin- en eindtijdvak en optioneel filteren op locatie en minimumcapaciteit.
- **FR-003**: Het systeem MUST een reservering alleen accepteren wanneer de eindtijd
  na de begintijd ligt.
- **FR-004**: Het systeem MUST nieuwe en gewijzigde reserveringen standaard alleen
  voor een toekomstig tijdvak accepteren; deze beleidskeuze is een labvoorstel tot
  BIDN haar bevestigt.
- **FR-005**: Het systeem MUST actieve overlappende reserveringen voor dezelfde ruimte
  voorkomen, ook wanneer aanvragen gelijktijdig binnenkomen.
- **FR-006**: Het systeem MUST aansluitende tijdvakken toestaan wanneer het gekozen
  beleid geen buffer voorschrijft; een buffer is een open productbesluit.
- **FR-007**: Het systeem MUST een reservering aan de ingelogde medewerker koppelen
  en mag de eigenaar niet uit vrije invoer van de medewerker overnemen.
- **FR-008**: Het systeem MUST medewerkers hun eigen toekomstige actieve reserveringen
  laten wijzigen, met opnieuw controle van tijd, ruimte en overlap.
- **FR-009**: Het systeem MUST een afgewezen wijziging verwerken zonder de bestaande
  reservering te veranderen.
- **FR-010**: Het systeem MUST medewerkers hun eigen toekomstige actieve
  reserveringen laten annuleren zonder de reservering uit de historie te verwijderen.
- **FR-011**: Het systeem MUST geannuleerde reserveringen niet als blokkade voor
  beschikbaarheid behandelen en het vrijgekomen tijdvak opnieuw beschikbaar maken.
- **FR-012**: Het systeem MUST eigen toekomstige, afgelopen en geannuleerde
  reserveringen afzonderlijk raadpleegbaar maken.
- **FR-013**: Het systeem MUST voorkomen dat medewerkers persoonsgegevens of
  reserveringsdetails van andere medewerkers via beschikbaarheid of reserverings-ID
  inzien.
- **FR-014**: Het systeem MUST beheerders ruimtes laten toevoegen, naam, positieve
  capaciteit en locatie laten aanpassen en ruimtes laten deactiveren wanneer geen
  lopende of toekomstige actieve reservering bestaat.
- **FR-015**: Het systeem MUST inactieve ruimtes uitsluiten van nieuwe
  beschikbaarheids- en boekingsresultaten, terwijl relevante historie behouden blijft.
- **FR-016**: Het systeem MUST beheeracties van onbevoegde medewerkers weigeren en
  geen wijziging uitvoeren.
- **FR-017**: Het systeem MUST tijden begrijpelijk tonen in Europe/Amsterdam en
  ongeldige of dubbelzinnige lokale tijden expliciet afhandelen.
- **FR-018**: Het systeem MUST bij validatie-, conflict-, autorisatie- en tijdelijke
  beschikbaarheidsfouten een begrijpelijke melding tonen zonder gevoelige details.
- **FR-019**: Het systeem MUST de status en historie van reserveringen behouden na
  annulering en mag reserveringen of gerefereerde ruimtes niet hard verwijderen.
- **FR-020**: Het systeem MUST requirements, user stories, scenario's en tests
  traceerbaar houden gedurende verdere uitwerking.

### Open Product Decisions and Lab Proposals

De volgende punten zijn herkenbaar als voorstel of besluitpunt en zijn niet
stilzwijgend goedgekeurd:

- **Labvoorstel**: nieuwe en gewijzigde reserveringen starten in de toekomst.
- **Labvoorstel**: aansluitende tijdvakken zijn toegestaan zonder buffer.
- **Open productbesluit**: zijn buffers, openingstijden, maximale duur of een
  boekingshorizon nodig?
- **Open productbesluit**: mag een beheerder reserveringen van andere medewerkers
  beheren, en welke gegevens mag die rol zien?
- **Open productbesluit**: hoe worden reserveringen beheerd bij afwezigheid van de
  eigenaar of bij een lopende reservering?
- **Open productbesluit**: welke bewaartermijn en privacyregels gelden voor historie?
- **Bronstatus**: `StakeholderDocuments/OpenQuestions.md` was niet aanwezig; de
  resterende open punten uit `TechStack.md` §15 moeten door BIDN worden bevestigd.

### Key Entities *(include if feature involves data)*

- **Medewerker**: persoon die beschikbaarheid bekijkt en eigen reserveringen maakt
  en beheert.
- **Beheerder**: bevoegde persoon die de vergaderruimtecatalogus onderhoudt; extra
  bevoegdheden voor reserveringen van anderen zijn niet vanzelfsprekend.
- **Vergaderruimte**: boekbare ruimte met naam, positieve capaciteit, locatie en een
  actieve of inactieve status.
- **Reservering**: tijdgebonden vastlegging van één vergaderruimte voor één eigenaar,
  met een actieve of geannuleerde status en raadpleegbare historie.
- **Tijdvak**: een begin- en eindtijd waarop beschikbaarheid en overlap worden
  beoordeeld.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In een testset met minimaal 20 geldige zoekopdrachten toont het systeem
  in 100% van de gevallen alleen ruimtes die het volledige gevraagde tijdvak vrij
  hebben en aan de gekozen filters voldoen.
- **SC-002**: In minimaal 100 herhaalde paren van gelijktijdige overlappende
  aanvragen voor dezelfde ruimte slaagt nooit meer dan één aanvraag per paar.
- **SC-003**: Medewerkers kunnen in een begeleide gebruikerstest van maximaal 3
  minuten een ruimte zoeken en een geldige reservering bevestigen; minimaal 9 van 10
  deelnemers voltooien dit zonder hulp.
- **SC-004**: In de autorisatietest worden alle pogingen van een andere medewerker om
  een reservering te lezen, wijzigen of annuleren geweigerd zonder persoonsgegevens
  prijs te geven.
- **SC-005**: Alle beschreven scenario's voor wijzigen, annuleren, historie,
  aansluitende tijdvakken, ongeldige tijden en afwijzing van conflicterende wijzigingen
  hebben een geslaagde geautomatiseerde acceptatietest vóór de release wordt
  gedemonstreerd.
- **SC-006**: In een toegankelijkheidscontrole kunnen gebruikers alle release-1-
  workflows met toetsenbordbediening uitvoeren en vinden zij voor elke ingevoerde
  fout een begrijpelijke tekstuele uitleg.
- **SC-007**: Een geannuleerde reservering blijft in 100% van de historiecontroles
  zichtbaar voor de eigenaar en blokkeert in 100% van de controles het tijdvak niet
  langer.
- **SC-008**: De release bevat geen gebruikersflow of integratie voor werkplekken,
  herhaalboekingen, notificaties, deelnemerslijsten, check-in/check-out, Outlook,
  Teams, Microsoft Graph, rapportages of capaciteitsanalyse.

## Assumptions

- De primaire gebruikers zijn BIDN-medewerkers en ruimtebeheerders; een betrouwbare
  inlogcontext bestaat voor de gebruikersflows.
- Release 1 ondersteunt uitsluitend vergaderruimtes als gebruikersfunctionaliteit.
  Werkplekken mogen niet zichtbaar of boekbaar worden gemaakt via een voorbereid
  uitbreidingsmodel.
- Een actieve reservering blokkeert een overlappend tijdvak; een geannuleerde
  reservering doet dat niet.
- Zonder bevestigd bufferbeleid zijn intervallen die exact op elkaar aansluiten
  toegestaan. Dit blijft een labvoorstel.
- Zonder bevestigd boekingsbeleid wordt een reservering met een start in het verleden
  of heden afgewezen. Dit blijft een labvoorstel.
- De lokale interface gebruikt Europe/Amsterdam voor invoer en weergave; ongeldige
  zomertijdgevallen worden niet stilzwijgend aangepast.
- De trainingsdemo gebruikt synthetische gegevens en vormt geen productieacceptatie.
- De ontbrekende `OpenQuestions.md` kan aanvullende besluiten bevatten; totdat die
  bron beschikbaar is, zijn de hierboven gemarkeerde open productbesluiten niet
  gesloten.
- Persoonsgegevens van andere medewerkers zijn niet nodig om beschikbaarheid te
  begrijpen en worden daarom niet getoond.

## Out of Scope

De volgende uitbreidingen worden uitsluitend als buiten scope geregistreerd en zijn
geen onderdeel van release 1:

- Werkplekfunctionaliteit als gebruikersflow.
- Herhaal- of terugkerende reserveringen.
- Notificaties en herinneringen.
- Deelnemerslijsten, vergadertitels of uitgebreide omschrijvingen.
- Check-in en check-out.
- Outlook-, Teams- of Microsoft Graph-integraties.
- Rapportages en capaciteitsanalyse.
- Productieacceptatie, definitieve hosting en andere operationele uitwerking.
