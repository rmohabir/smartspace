SmartSpace — Reserveringssysteem voor vergaderruimtes en werkplekken

Status en gebruik

Techstack, systeemontwerp en bouwinstructie voor het SmartSpace-trainingsprototype bij BIDN. Dit document vervangt de RSS Feed Reader-casus en is bruikbaar als input voor Spec-Driven Development en AI-assisted implementatie. De applicatie is met dit document nog niet gebouwd of getest. Het document is ontwerpinput en geen goedgekeurde productspecificatie.

De eerste release richt zich op vergaderruimtes. Werkplekken worden meegenomen in het datamodel, maar nog niet als gebruikersfunctionaliteit gebouwd. Aanvullende beleidskeuzes zijn hieronder expliciet als voorstel gemarkeerd; ze zijn nog niet door BIDN gevalideerd.

1. Achtergrond en doelstelling

Binnen BIDN bestaat behoefte aan een centrale oplossing voor het reserveren van vergaderruimtes en op termijn flexwerkplekken. De AI-training heeft daarmee een daadwerkelijk bedrijfsdoel.

Deelnemers werken gezamenlijk aan een te valideren productspecificatie, een eerste systeemontwerp, een backlog, een technisch prototype en vastgelegde architectuur- en ontwerpbeslissingen. De resultaten moeten bruikbaar blijven als startpunt voor eventuele realisatie binnen BIDN.

SmartSpace stelt medewerkers in staat beschikbare ruimtes te raadplegen, reserveringen aan te maken, te wijzigen en te annuleren, en eigen reserveringen inclusief historie te bekijken. Beheerders configureren ruimtes, capaciteit en locatie.

2. Techstack

Onderdeel

Keuze

Toepassing

Framework

.NET 10, net10.0

Alle C#-projecten

Frontend

Standalone Blazor WebAssembly

Interactieve Razor-componenten in de browser

Styling

Tailwind CSS 4 via lokale CLI

Responsive dashboard en formulieren

Backend

ASP.NET Core 10 Minimal API

HTTP/JSON-endpoints, geen controllers

Data access

Entity Framework Core 10

Query's, relaties, transacties en opslag

Database prototype

SQLite in-memory

Relationele tijdelijke opslag in het backendproces

Contracten

Gedeelde C# DTO-library

Requests, responses en foutcodes

Identiteit prototype

Development-only testidentiteit

Eigen reserveringen en rollen demonstreren

Identiteit bedrijfsgebruik

Voorstel: Microsoft Entra ID

Afstemmen op BIDN-identiteitsarchitectuur

Verificatie

xUnit en ASP.NET Core-integratietests

Businessregels, autorisatie en concurrency

‘Northwind’ wordt uitsluitend geïnterpreteerd als de eerder gewenste rustige dashboardstijl. Er is geen specifiek template geselecteerd. Gebruik Tailwind voor vormgeving en Blazor voor interactie, zonder extra componentframework of Northwind-voorbeelddatabase.

SQL is geen backendframework: Minimal API is de HTTP-laag en SQLite is de database. EF Core draait alleen op de backend. De browser benadert de database nooit rechtstreeks.

Kritisch onderscheid: prototype en bedrijfsrelease

SQLite in-memory verliest alle gegevens zodra de database wordt gesloten of het proces stopt. Daarmee is de eis ‘historische reserveringen blijven raadpleegbaar’ alleen binnen één prototypesessie aantoonbaar. Voor een echte eerste bedrijfsrelease is duurzame opslag noodzakelijk. De gevraagde in-memory stack blijft dus behouden voor de training, met deze expliciete beperking.

Een vervolgstap kan SQLite op schijf voor een beperkte proef zijn. De uiteindelijke BIDN-databasekeuze, back-up, herstel en hosting moeten als architectuurbesluit worden vastgesteld. Een overstap naar SQL Server/Azure SQL vraagt provider-specifieke migrations en opnieuw geteste concurrency; alleen de connection string wijzigen is onvoldoende.

3. Eerste release en afbakening

Gebied

Binnen eerste release

Beschikbaarheid

Vergaderruimtes zoeken voor een gekozen begin- en eindtijd, met locatie- en capaciteitsfilter

Reserveren

Een beschikbare vergaderruimte vastleggen

Wijzigen

Ruimte en/of tijd aanpassen, met nieuwe overlapcontrole

Annuleren

Reservering annuleren en historie behouden

Eigen overzicht

Toekomstige, afgelopen en geannuleerde eigen reserveringen

Conflicten

Dubbele boekingen verhinderen, ook bij gelijktijdige requests

Beheer

Ruimtes aanmaken, wijzigen en deactiveren; capaciteit en locatie onderhouden

Niet bouwen in deze release: werkplekboekingen, check-in/check-out, Outlook, Teams, Microsoft Graph, rapportages, capaciteitsanalyse, terugkerende boekingen en notificaties. Neem passende uitbreidingsmogelijkheden op, zonder alvast externe integraties of achtergrondprocessen toe te voegen.

4. Architectuur en verantwoordelijkheden

De Blazor-frontend roept de Minimal API aan via een getypeerde HttpClient. Endpoints verwerken DTO's en geven de werkzaamheden door aan services. Services handhaven businessregels en gebruiken een scoped EF Core DbContext.

Frontend: zoeken en filteren, reserveringsformulieren, eigen overzicht, beheerpagina's en toegankelijke laad-/foutmeldingen. Clientvalidatie helpt de gebruiker; servervalidatie blijft beslissend.

Backend: identiteit en rollen controleren, beschikbaarheid berekenen, eigenaarschap afdwingen, wijzigingen atomair uitvoeren en conflicten als betekenisvolle HTTP-responses teruggeven.

Database: resources, locaties en reserveringen opslaan; foreign keys en basisconstraints afdwingen. Een unieke index op ruimte en begintijd voorkomt geen willekeurige intervaloverlap.

Gebruik één applicatie met duidelijke feature-indeling. Voeg voor deze scope geen microservices, generieke repositorylaag of verplichte CQRS/MediatR toe. Houd de reserveringslogica wel buiten Program.cs, zodat deze gericht testbaar is.

Projectstructuur

Pad

Inhoud

backend/SmartSpace.Api/Endpoints/

Rooms, Reservations en Administration

backend/SmartSpace.Api/Services/

BookingService, AvailabilityService, RoomService

backend/SmartSpace.Api/Data/

SmartSpaceDbContext en entityconfiguraties

backend/SmartSpace.Api/Models/

Resource, Location en Reservation

backend/SmartSpace.Api/Security/

Rollen, policies en development-identiteit

frontend/SmartSpace.UI/Pages/

Beschikbaarheid, eigen reserveringen, beheer

frontend/SmartSpace.UI/Components/

Filters, kaarten, formulieren, statusweergave

frontend/SmartSpace.UI/Layout/

Desktop- en mobiele navigatie

frontend/SmartSpace.UI/Services/

Getypeerde API-clients

frontend/SmartSpace.UI/Styles/

Tailwind-bronbestand

shared/SmartSpace.Contracts/

DTO's en stabiele foutcodes, zonder EF

tests/SmartSpace.Api.Tests/

Integratie-, autorisatie- en concurrencytests

docs/adr/

Architectuurbesluiten en gevolgen

5. Datamodel

Location

Id, Name, optioneel Building en Floor, plus IsActive.

Gebruik locatie-ID's om inconsistent gespelde locaties te vermijden. Een locatie waaraan ruimtes gekoppeld zijn wordt niet fysiek verwijderd. Zelfstandig locatiebeheer is een kleine ondersteunende beheertaak; seed locaties als dit niet in de trainingsplanning past.

Resource

Id, Name, ResourceType, Capacity, LocationId, IsActive en een applicatiebeheerde Version (Guid-concurrencytoken).

ResourceType ondersteunt conceptueel MeetingRoom en Workstation. De eerste release accepteert en toont uitsluitend MeetingRoom. Houd endpoints en UI herkenbaar als ‘vergaderruimtes’; de generieke kern voorkomt dat Reservation later alleen naar een Room kan verwijzen.

Capaciteit moet minimaal 1 zijn. Naam en locatie zijn verplicht. Voorstel: maak ruimtenamen uniek binnen dezelfde locatie. Het model ondersteunt later uitbreiding zonder nu een generiek resourceplatform te bouwen.

Reservation

Id, ResourceId, OwnerSubjectId, StartUtc, EndUtc, Status (Active of Cancelled), CreatedAtUtc, UpdatedAtUtc, optioneel CancelledAtUtc en Version (Guid-concurrencytoken).

‘Afgelopen’ is een afgeleide weergave op basis van EndUtc, geen extra status die een achtergrondtaak moet bijwerken. Voeg geen verplichte vergadertitel, deelnemerslijst of omschrijving toe zonder productbesluit: die zijn niet nodig voor de opgegeven scope.

Indexeer ResourceId, Status en StartUtc voor beschikbaarheidsquery's, en OwnerSubjectId met StartUtc voor het eigen overzicht. Leg foreign keys en een databasecheck EndUtc > StartUtc vast. Bewaar tijden als UTC DateTime in SQLite; gebruik DTO's met expliciete offset en normaliseer op de server.

Annuleren wijzigt de status en verwijdert de rij niet. Deactiveer gebruikte ruimtes in plaats van ze te verwijderen. Zo blijven historische reserveringen aan hun ruimte gekoppeld. Een volledig wijzigingslog en historische snapshots van ruimtenamen zijn aanvullende eisen en worden niet stilzwijgend verondersteld.

6. Businessregels

ID

Regel

Status

BR-01

Een reservering heeft een begin- en eindtijd; eindtijd ligt na begintijd

Opgegeven eis

BR-02

Actieve reserveringen voor dezelfde resource mogen niet overlappen

Opgegeven eis

BR-03

De overlapcontrole geldt zowel bij aanmaken als wijzigen

Uitwerking van eis

BR-04

Geannuleerde reserveringen blokkeren geen beschikbaarheid

Ontwerpvoorstel

BR-05

Historische en geannuleerde reserveringen worden niet fysiek verwijderd

Uitwerking van historie-eis

BR-06

Aansluitende boekingen zijn toegestaan, bijvoorbeeld 10–11 en 11–12

Ontwerpvoorstel: geen buffer

BR-07

Medewerkers kunnen alleen hun eigen reserveringen wijzigen/annuleren

Autorisatievoorstel

BR-08

Nieuwe reserveringen starten in de toekomst; begonnen en afgelopen reserveringen zijn alleen-lezen

Beleidsvoorstel, te valideren

BR-09

Alleen actieve ruimtes op actieve locaties zijn nieuw te reserveren

Ontwerpvoorstel

BR-10

Deactiveren met toekomstige actieve boekingen wordt geweigerd totdat die zijn opgelost

Beleidsvoorstel; geen stilzwijgende annulering

Voorgestelde standaard: geen minimale duur, maximale duur, boekingshorizon of openingstijden afdwingen totdat BIDN die heeft vastgesteld. Filteren op capaciteit betekent een minimumaantal zitplaatsen; het introduceert geen deelnemersregistratie.

Overlapberekening

Gebruik halfopen intervallen [start, end). Twee reserveringen overlappen wanneer:

existing.StartUtc < requestedEndUtc &&
existing.EndUtc > requestedStartUtc

Filter daarbij op dezelfde ResourceId en Status Active. Sluit bij wijzigen de eigen ReservationId uit. De backend voert dit ook uit wanneer de UI de ruimte eerder als beschikbaar toonde: beschikbaarheid is geen tijdelijke claim of garantie.

Gelijktijdige boekingen

Alleen eerst AnyAsync en daarna SaveChangesAsync uitvoeren is onveilig: twee requests kunnen beide ‘vrij’ lezen.

Voor SQLite moet één write-transactie het gehele traject omvatten: write-lock verkrijgen, actuele resource en reserveringen lezen, rechten/versie/tijden/overlap controleren, opslaan en committen. Gebruik een immediate SQLite-transactie (via de provider ondersteunde non-deferred transactie) en koppel EF Core expliciet aan diezelfde verbinding/transactie. Begin de overlapcontrole pas nadat de lock is verkregen.

Voer ook wijzigen, annuleren en deactiveren via de afgesproken transactieaanpak uit. Houd transacties kort en voer er geen netwerkcalls in uit. Behandel SQLITE_BUSY/SQLITE_LOCKED met begrensde wachttijd of begrensde retries van de volledige operatie; iedere retry leest en valideert opnieuw. Na uitputting volgt 503, geen onjuiste 409. Een geconstateerde overlap geeft 409. Gebruik geen blinde retry na een onbekende commituitkomst.

Voeg daarnaast een applicatiebeheerd concurrencytoken toe om overschrijven door een verouderd formulier te verhinderen. De client stuurt de gelezen Version mee bij wijzigen/annuleren; de server vergelijkt die en vervangt het token bij succes. Een concurrencytoken op bestaande rijen voorkomt op zichzelf geen dubbele nieuwe boekingen.

Een process-local lock alleen is geen voldoende basis voor een later gedeelde database met meerdere API-instances. Leg bij provider- of hostingwijzigingen opnieuw vast hoe intervaloverlap transactioneel wordt verhinderd.

Tijdzones

Gebruik Europe/Amsterdam als expliciete weergave- en invoertijdzone voor het prototype. Stuur ISO 8601 met offset naar de API en normaliseer naar UTC. Toon de tijdzone in de UI. Valideer ongeldige tijden tijdens de zomertijdsprong en vraag bij dubbel voorkomende lokale tijden een expliciete keuze; gebruik nooit ongemerkt de servertijdzone. Injecteer TimeProvider voor reproduceerbare tijdsafhankelijke tests.

7. Identiteit en autorisatie

‘Eigen reserveringen’ vereist een betrouwbare gebruikersidentiteit. Leid OwnerSubjectId af uit server-side gevalideerde claims, nooit uit een vrij invulbaar userId in de request-body.

Handeling

Medewerker

Beheerder

Ruimtes en beschikbaarheid bekijken

Ja

Ja

Eigen reservering aanmaken/bekijken/wijzigen/annuleren

Ja

Ja

Reservering van een ander inzien of beheren

Nee

Niet standaard; apart beleidsbesluit

Ruimtes, capaciteit en locatie configureren

Nee

Ja

De beheerdersrol betekent dus niet automatisch toegang tot alle reserveringsdetails. Beschikbaarheid kan bezette tijdvakken tonen zonder de eigenaar te delen.

Gebruik voor training een development-only authenticatiehandler met vaste testpersonen en claims, waarmee minimaal twee medewerkers en één beheerder kunnen worden getest. Een eventuele testpersoonselectie is uitsluitend beschikbaar bij expliciete Development-configuratie. Buiten Development moet startup weigeren als demo-authenticatie is ingeschakeld. Een keuzevakje of header met een gebruikersnaam is geen productieauthenticatie.

Voor bedrijfsgebruik: werk Entra ID-login, API-tokenvalidatie, tenantrestrictie en roltoewijzing uit volgens BIDN-afspraken. Autorisatie wordt op elk endpoint en in de mutatielogica afgedwongen; een verborgen beheerknop is onvoldoende.

8. Minimal API-contract

Methode

Route

Gedrag

GET

/api/locations

Beschikbare locaties voor filters

GET

/api/rooms

Ruimtecatalogus; filters op locatie en minimumcapaciteit

GET

/api/rooms/availability?start=...&end=...&locationId=...&minCapacity=...

Beschikbare ruimtes voor het exacte interval

GET

/api/rooms/{id}

Ruimtedetails

GET

/api/reservations/mine?view=upcoming&page=1&pageSize=20

Eigen reserveringen; views upcoming, past, cancelled

GET

/api/reservations/{id}

Eigen reservering en actuele Version

POST

/api/reservations

Aanmaken; 201 met Location-header en DTO

PUT

/api/reservations/{id}

Ruimte/tijden wijzigen met Version; 200

POST

/api/reservations/{id}/cancel

Annuleren met Version; 200

POST

/api/admin/rooms

Ruimte aanmaken; 201

PUT

/api/admin/rooms/{id}

Configuratie wijzigen met Version; 200

POST

/api/admin/rooms/{id}/deactivate

Deactiveren met Version; 200 of beleidsconflict

Voorstel: PUT op een kamer ondersteunt ook heractiveren. Lever de beheerweergave inclusief inactieve ruimtes via een aanvullend beheer-GET, zonder inactieve ruimtes in de normale beschikbaarheidsresultaten op te nemen.

CreateReservationRequest bevat resourceId, start en end. Update bevat dezelfde velden plus version. Cancel bevat version. Eigenaar en status worden door de server bepaald. Maak herhaalde annulering van een al geannuleerde eigen reservering zonder neveneffecten succesvol; een eerste annulering controleert wel het concurrencytoken.

Standaardfouten: 400 invoer ongeldig, 401 niet ingelogd, 403 onvoldoende rol, 404 niet gevonden of een niet-toegankelijke reservering, 409 overlap/verouderde versie/beleidsconflict, 503 tijdelijke databasebezetting. Gebruik ProblemDetails met een stabiele code zoals booking_overlap of stale_version. Toon korte Nederlandstalige meldingen en geen interne exceptiondetails.

Gebruik expliciete validatie op de server, CancellationToken, async EF-calls en AsNoTracking voor reads. Begrens pageSize en sorteer lijsten stabiel op tijd en ID. De precieze filter- en DTO-contracten worden vóór implementatie als OpenAPI/contractdocument vastgelegd.

9. SQLite in-memory configuratie

Gebruik Microsoft.EntityFrameworkCore.Sqlite 10.0.x met onderling afgestemde Microsoft-packageversies. Gebruik niet de EF InMemory-provider: die is geen relationele SQLite-database.

Data Source=SmartSpace;Mode=Memory;Cache=Shared;Pooling=False

Houd gedurende de API-levensduur een keeper-connection open. Geef iedere request een eigen scoped DbContext met dezelfde connection string. Deel geen DbContext of actief gebruikte databaseverbinding tussen requests. Maak het schema bij startup aan met EnsureCreatedAsync en seed uitsluitend fictieve ruimtes, locaties en testpersonen.

De keeper voorkomt verlies tussen requests; deze maakt de database niet duurzaam. Meerdere API-processen hebben ieder een eigen in-memory database en mogen voor deze demo niet als één gedeeld reserveringssysteem worden gepresenteerd.

Gebruik bij tests een unieke databasenaam per geïsoleerd scenario. Voor concurrencytests gebruiken de gelijktijdige clients juist dezelfde testdatabase. Bij overgang naar duurzame opslag worden EF-migrations, herstel en back-up expliciet ingericht.

10. Moderne responsive UI

Ontwerpstijl

Een zakelijke, rustige SmartSpace-workspace: donkere slate-950-zijbalk, slate-50-paginaachtergrond, witte kaarten, indigo-600-primaire acties en subtiele borders. Gebruik consistente 8-px-afstanden, ronde hoeken van circa 16 px, systeemtypografie en herkenbare lokale SVG-iconen voor ruimte, locatie, personen en tijd.

De UI is Nederlandstalig. Toon echte gegevens; geen verzonnen bezettingspercentages of niet-werkende knoppen voor toekomstige features.

Pagina 1 — Ruimte zoeken (/)

Titel ‘Vind een ruimte voor je overleg’. Bovenaan een filterpaneel met datum, begin- en eindtijd, locatie en minimaal aantal personen, gevolgd door ‘Zoek beschikbare ruimtes’.

Toon pas een beschikbaarheidslabel nadat een geldig interval is gekozen. Een ruimte is niet in algemene zin ‘beschikbaar’, maar alleen voor de geselecteerde periode.

Resultaten verschijnen als kaarten met ruimtenaam, locatie, capaciteit, geselecteerd tijdvak en ‘Reserveren’. Voeg duidelijke toestanden toe voor eerste bezoek, laden, geen passende ruimtes, ongeldige tijden en netwerkfout. ‘Geen resultaten’ is geen technische fout.

Pagina 2 — Reserveren/wijzigen

Gebruik een routeerbaar formulier, zodat mobiel en toetsenbordgebruik eenvoudig blijven. Toon ruimte, locatie en tijden in een heldere samenvatting. Het formulier ondersteunt tijd- of ruimtewijziging en valideert opnieuw op de server.

Tijdens opslaan is de knop tijdelijk uitgeschakeld. Na succes volgt bevestiging en een link naar ‘Mijn reserveringen’. Bij een overlap blijven ingevulde gegevens staan en verschijnt ‘Deze ruimte is zojuist gereserveerd. Kies een ander tijdstip of een andere ruimte.’ Bij een stale version laat de UI de actuele versie opnieuw ophalen voordat de gebruiker een nieuwe wijziging indient.

Pagina 3 — Mijn reserveringen (/mijn-reserveringen)

Tabs ‘Komend’, ‘Afgelopen’ en ‘Geannuleerd’. Desktop toont overzichtelijke rijen; mobiel kaarten. Elke reservering bevat ruimte, locatie, datum, begin/eind en tekstuele status. Wijzigen en annuleren zijn alleen zichtbaar wanneer toegestaan. Annuleren vraagt een korte bevestiging met de concrete ruimte en tijd. Historie blijft zichtbaar.

Pagina 4 — Beheer (/beheer/ruimtes)

Alleen voor beheerders. Toon naam, locatie, capaciteit en actief/inactief. Bied toevoegen, bewerken en deactiveren. Gebruik inline validatie en leg bij een blokkade uit dat toekomstige reserveringen eerst opgelost moeten worden. De eerste release geeft beheerders niet stilzwijgend het recht om boekingen van anderen te annuleren.

Responsiviteit en toegankelijkheid

Scherm

Gedrag

Onder 640 px

Eén kolom, compacte header, mobiele navigatie, gestapelde filters, grote actieknoppen

640–1023 px

Twee kaartkolommen waar dit past, filters in overzichtelijke rijen

Vanaf 1024 px

Zijbalk circa 240 px, ruime hoofdsectie, twee of drie kaartkolommen

Controleer 375, 768 en 1440 px en 200% zoom. Voorkom horizontale overflow. Gebruik zichtbare labels, keyboardfocus, circa 44-px-touchdoelen, foutmeldingen bij velden en aria-live voor statusberichten. Communiceer status met tekst naast kleur. Een mobiele navigatie-overlay ondersteunt Escape en focusherstel. Forceer geen complex kalenderrooster voor het prototype: tijdfilters en resultatenkaarten dekken de eerste scope.

11. Tailwind-configuratie

Voer uit in frontend/SmartSpace.UI:

npm install -D tailwindcss@4 @tailwindcss/cli@4

Maak Styles/app.css:

@import "tailwindcss";
@source "../Pages";
@source "../Components";
@source "../Layout";
@source "../wwwroot/index.html";

Voeg scripts toe aan package.json:

{
  "scripts": {
    "css:watch": "npx @tailwindcss/cli -i ./Styles/app.css -o ./wwwroot/css/app.css --watch",
    "css:build": "npx @tailwindcss/cli -i ./Styles/app.css -o ./wwwroot/css/app.css --minify"
  }
}

Laad css/app.css in wwwroot/index.html en behoud de viewport-metatag. Commit package-lock.json. Gebruik complete utility-classnamen in Razor en geen dynamisch samengestelde fragmenten. Verwijder Bootstrap en conflicterende template-/layout-CSS. Bouw CSS vóór dotnet publish en verifieer ook de publish-output. JavaScript is alleen nodig wanneer een concrete browserinteractie dat vereist; de componentlogica blijft in Blazor.

12. Lokale ontwikkeling

Instelling

Waarde

API-project

backend/SmartSpace.Api/SmartSpace.Api.csproj

UI-project

frontend/SmartSpace.UI/SmartSpace.UI.csproj

API development-URL

http://localhost:5151

UI development-URL

http://localhost:5213

UI ApiBaseUrl

http://localhost:5151/api/

API CORS-origin

http://localhost:5213

Maak expliciete lokale HTTP-launchprofielen. Lees de API-URL uit frontendconfiguratie; plaats daar geen secrets. Gebruik relatieve clientpaden zonder voorloopslash zodat /api/ behouden blijft. Bij echte authenticatie worden de HTTPS-adressen, redirect-URI's en API-audience samen afgestemd.

Verwijder ongebruikte demo-pagina's en navigatielinks. Zorg dat precies één pagina @page "/" heeft en dat alle andere routes uniek zijn. Maak eerst een werkende applicatieshell, daarna de features. Bestandsnamen uit templates kunnen verschillen; het doel is unieke routes en geen demonstratiepagina's.

Start Tailwind-watch, API en UI als drie lokale processen. Documenteer de exacte commando's in de README. Controleer CORS, rootroute en API-bereikbaarheid in de browser vóór de verdere UI-bouw.

13. Bouwfasen en initiële backlog

Fase

Backlogitem

Resultaat

0 — Specificeren

Scope, beleidsvoorstellen en autorisatiematrix valideren

Geaccordeerde regels en open besluiten

1 — Foundation

Projecten, Tailwind-shell, demo-identiteit, EF-schema, poorten

Startbare lokale basis

2 — Beschikbaarheid

Ruimtes/filtering en tijdvakquery

Alleen passende vrije ruimtes

3 — Reserveren

Aanmaken met transactie en conflictgedrag

Geen dubbele boeking onder concurrency

4 — Eigen beheer

Mijn reserveringen, wijzigen, annuleren, historie

Volledige medewerkerflow

5 — Ruimtebeheer

Configuratie, capaciteit, locatie, deactivering

Rolbeveiligde beheerflow

6 — Valideren

Integratietests, responsive controles, documentatie

Aantoonbaar werkend prototype

Iedere backlogtaak verwijst naar een requirement-ID, relevante businessregel, API-contract en acceptatiescenario. Leg in het trainingsdossier onderscheid vast tussen gevraagde eis, ontwerpvoorstel en daadwerkelijk gevalideerde beslissing. ‘Gevalideerde productspecificatie’ mag pas als status worden gebruikt na beoordeling door de betrokken BIDN-stakeholders.

Voor architectuurbesluiten: ADR-001 stack en applicatiegrenzen; ADR-002 tijdelijke versus duurzame opslag; ADR-003 overlap en transacties; ADR-004 identiteit/rollen; ADR-005 tijdzone; ADR-006 Resource-model en toekomstige werkplekken.

14. Acceptatie en verificatie

Scenario

Verwacht resultaat

Geldige aanvraag voor vrije ruimte

201; zichtbaar in eigen overzicht

Eindtijd gelijk aan of vóór begintijd

400; niets opgeslagen

Gedeeltelijke overlap, insluiting of identiek tijdvak

409 voor dezelfde ruimte

Aansluitende intervallen

Beide toegestaan volgens BR-06

Zelfde tijdvak, verschillende ruimtes

Beide toegestaan

Twee gelijktijdige overlappende aanvragen

Hoogstens één slaagt; tweede krijgt na hercontrole 409, of tijdelijk 503 bij uitgeputte lock-wachttijd

Wijzigen naar bezet tijdvak

409; oorspronkelijke reservering ongewijzigd

Verouderd formulier opslaan

409; geen verloren wijziging

Annuleren

Status Cancelled, tijdvak opnieuw beschikbaar, historie behouden

Andere medewerker gebruikt reserverings-ID

Geen inzage of wijziging; 404 volgens API-beleid

Medewerker roept beheerendpoint aan

403

Niet-ingelogde request

401

Inactieve ruimte boeken

Afwijzen; niet in vrije resultaten

Ruimte deactiveren met toekomstige boekingen

Beleidsconflict; bestaande boekingen behouden

Zomer-/wintertijdgrens

Geen stilzwijgend verkeerd tijdstip

API herstart, in-memory profiel

Data weg; beperking expliciet gedocumenteerd

Duurzaam profiel vóór bedrijfsrelease

Historie blijft beschikbaar na herstart en herstel is aantoonbaar

Voer de concurrencytest met afzonderlijke requests en DbContexts tegen dezelfde SQLite-database uit. Assert niet alleen responses maar ook dat geen overlappende actieve rijen bestaan. Gebruik geen gemockte repository als bewijs voor databaseconcurrency. Test de twee medewerkers en beheerder op API-niveau en controleer de belangrijkste flows in de browser.

Het prototype is gereed wanneer alle medewerker- en beheerflows op de echte API werken, de kritieke regels aantoonbaar worden afgedwongen, de UI responsive is en beperkingen/open besluiten helder zijn vastgelegd. Dit is nog geen verklaring van productierijpheid.

15. Open besluiten vóór bedrijfsgebruik

Wie mag wijzigen/annuleren, ook bij lopende boekingen en afwezigheid van de eigenaar?

Mag een beheerder boekingen van anderen beheren en welke details zien collega's?

Zijn buffers, openingstijden, maximale duur en boekingshorizon nodig?

Hoe moeten ruimtewijzigingen en volledige wijzigingshistorie zichtbaar blijven?

Welke duurzame database, identiteitskoppeling en hosting passen bij BIDN?

Wie is eigenaar van het product, ruimtebeheer en de acceptatie van de trainingsresultaten?

Technische bronnen

ASP.NET Core 10 API-overzicht

EF Core: SQLite in-memory en connection lifetime

EF Core: beperkingen van SQLite, tijdtypen en concurrency

EF Core: applicatiebeheerde concurrencytokens

SQLite: transacties en BEGIN IMMEDIATE

Tailwind CSS: CLI-build

De productregels met de status ‘voorstel’, de UI en projectindeling zijn ontwerpkeuzes voor SmartSpace; de bronnen vormen geen bewijs dat BIDN deze keuzes al heeft vastgesteld.