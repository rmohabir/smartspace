# SmartSpace stakeholderreview

## Status

Dit document is een review van de aangeleverde stakeholderdocumenten op 16 september 2026. Het is geen goedkeuring van de productspecificatie en bevat geen applicatiecode.

## Scope in het kort

De eerste release is een lokaal trainingsprototype voor vergaderruimtes. Medewerkers kunnen ruimtes zoeken, reserveren, hun eigen toekomstige en historische reserveringen bekijken, wijzigen en annuleren. Beheerders kunnen ruimtes configureren en deactiveren. Beschikbaarheid en reserveringen moeten ook bij gelijktijdige aanvragen correct blijven.

Buiten scope vallen werkplekken als gebruikersfunctie, check-in en check-out, terugkerende boekingen, notificaties, deelnemerslijsten, Outlook, Teams, Microsoft Graph, rapportages en capaciteitsanalyse. Werkplekken mogen wel conceptueel in het datamodel worden voorbereid; dat is geen toezegging om ze in release 1 te bouwen.

Het resultaat van de training bestaat uit specificatie-input, ontwerp, backlog, lokaal prototype, tests en architectuurbesluiten. Het prototype gebruikt synthetische data. Het is geen productieacceptatie.

## Oorspronkelijke businessregels

Deze regels komen herkenbaar terug in de stakeholderinput en vormen de te valideren basis voor de productscope:

- Een reservering heeft een begin- en eindtijd; de eindtijd ligt na de begintijd.
- Actieve reserveringen voor dezelfde ruimte mogen niet overlappen.
- Gelijktijdige aanvragen mogen geen dubbele actieve reservering opleveren.
- Een afgewezen wijziging laat de bestaande reservering intact.
- Annuleren bewaart historie en maakt het tijdvak opnieuw beschikbaar.
- Eigenaren kunnen hun eigen historische reserveringen raadplegen.
- Beschikbaarheidsinformatie deelt geen persoonsgegevens van andere medewerkers.
- De gebruikersinterface toont lokale tijden voor Europe/Amsterdam.
- Beheerders kunnen ruimtes onderhouden; zij krijgen niet automatisch inzage of mutatierecht op reserveringen van andere medewerkers.
- Werkplekken en externe integraties zijn niet onderdeel van release 1.

Deze opsomming is nog niet door BIDN als definitieve productbeslissing ondertekend. De term "oorspronkelijk" betekent hier: afkomstig uit de aangeleverde business- en scope-input, niet: formeel goedgekeurd.

## Labvoorstellen

De volgende punten zijn ontwerpkeuzes of technische voorstellen en mogen niet als vaststaande businessregels worden gelezen:

- Alleen reserveringen met status `Active` blokkeren beschikbaarheid; annuleringen worden niet verwijderd.
- Aansluitende intervallen zijn toegestaan door halfopen intervallen `[start, end)` en zonder buffer.
- Nieuwe of gewijzigde reserveringen moeten in de toekomst beginnen.
- Alleen actieve ruimtes en actieve locaties zijn nieuw te reserveren.
- Deactiveren wordt geweigerd zolang lopende of toekomstige actieve reserveringen bestaan.
- Ruimtenamen zijn uniek binnen een locatie.
- Een development-only authenticatiehandler gebruikt vaste testpersonen; bedrijfsgebruik zou Microsoft Entra ID vereisen.
- SQLite in-memory is geschikt voor het lokale prototype, maar niet voor duurzame bedrijfsopslag.
- Een SQLite-transactie met lock-wachttijd/retries en een applicatiebeheerd concurrencytoken wordt gebruikt om gelijktijdige mutaties te beheersen.
- Het Resource-model kan later werkplekken ondersteunen zonder die functionaliteit nu te bouwen.
- De voorgestelde .NET-, Blazor-, Minimal API-, EF Core- en Tailwind-stack is een trainingskeuze, geen vastgesteld BIDN-platformbesluit.

## Tegenstrijdigheden en verduidelijkingen

1. `AppFeatures.md` verwees naar `OpenQuestions.md`, maar dat bestand bestaat niet. De open besluiten zijn nu gekoppeld aan §15 van `TechStack.md`.
2. `TechStack.md` sprak op enkele plaatsen over een "gevalideerde productspecificatie", terwijl dezelfde documenten zeggen dat een productverantwoordelijke de regels nog moet valideren. De formulering is aangepast naar "te valideren".
3. `AppFeatures.md` presenteerde toekomststart en aansluitende boekingen als regels, terwijl `TechStack.md` ze als voorstellen markeerde. Ze zijn nu in `AppFeatures.md` als labvoorstellen gelabeld.
4. Historie is een scope-eis, maar SQLite in-memory bewaart data alleen zolang het prototypeproces blijft draaien. Duurzame historie is daarom een expliciet vereiste voor een bedrijfsrelease, niet een eigenschap van het trainingsprototype.
5. Werkplekken zijn buiten scope als gebruikersfunctie, maar komen wel terug als conceptueel ResourceType. Dat is alleen consistent als UI, endpoints en acceptatietests voor werkplekken uit release 1 blijven.
6. "Beheerder" is wel een rol voor ruimtebeheer, maar het is nog niet besloten of en wanneer die rol reserveringen van anderen mag beheren. De autorisatiematrix en de open besluiten moeten hierover leidend worden gemaakt.

## Ontbrekende besluiten vóór implementatie of bedrijfsgebruik

- Wie mag een reservering wijzigen of annuleren wanneer de eigenaar afwezig is of de reservering al loopt?
- Mag een beheerder reserveringen van anderen beheren, en welke gegevens zijn dan zichtbaar?
- Zijn buffers, openingstijden, maximale duur en een boekingshorizon nodig?
- Hoe worden zomer-/wintertijdgevallen en dubbel voorkomende lokale tijden beleidsmatig afgehandeld?
- Moeten naam- of locatiewijzigingen met terugwerkende kracht zichtbaar zijn in historische reserveringen?
- Welke duurzame database, back-up/herstelstrategie, hosting en identity-integratie zijn voor BIDN vereist?
- Wie is producteigenaar, wie beheert ruimtes en wie accepteert het trainingsresultaat?
- Welke API-contracten, foutcodes en statusovergangen worden definitief vastgesteld?

## Reviewgrens

Er is geen applicatiecode gegenereerd, geen voorstel is als goedgekeurd verklaard en alleen de stakeholderdocumentatie is verbeterd. Een volgende fase mag pas implementeren nadat de open besluiten door de bevoegde BIDN-stakeholders zijn bevestigd.
