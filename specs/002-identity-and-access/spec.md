# Feature Specification: SmartSpace identity and access

**Feature Branch**: `002-identity-and-access`

**Created**: 2026-09-23

**Status**: Draft

**Input**: User description: "Voeg een minimale, niet-complexe authenticatie- en autorisatiebasis toe voor SmartSpace met lokale DevelopmentIdentity, Entra ID-ready dev/prod, server-side eigenaarschap en Administrator app role."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Lokaal ontwikkelen met demo-identiteit (Priority: P1)

Als ontwikkelaar wil ik SmartSpace lokaal kunnen blijven starten zonder Entra tenantconfiguratie, zodat demo's en geautomatiseerde tests snel blijven werken.

**Why this priority**: De bestaande lokale workflow moet beschikbaar blijven terwijl de productieachtige identity boundary wordt voorbereid.

**Independent Test**: Start de API in Development met development identity actief. Een request met testsubject en rol wordt geaccepteerd, terwijl dezelfde configuratie buiten Development wordt geweigerd voordat de app start.

**Acceptance Scenarios**:

1. **Given** de API draait in Development en development identity is actief, **When** een request een testsubject meestuurt, **Then** ziet de API dat subject als gevalideerde gebruiker.
2. **Given** de API draait niet in Development en development identity is actief, **When** de app start, **Then** faalt startup met een duidelijke configuratiefout.
3. **Given** een lokaal request geen testsubject of bearer token bevat, **When** een beschermd endpoint wordt aangeroepen, **Then** krijgt de gebruiker geen toegang.

### User Story 2 - Server-side identiteit gebruiken voor eigenaarschap (Priority: P1)

Als medewerker wil ik dat SmartSpace mijn reserveringen koppelt aan mijn gevalideerde identiteit, zodat niemand via clientinvoer eigenaar kan worden van mijn reserveringen.

**Why this priority**: Eigenaarschap en privacy zijn kernregels voor reserveringen en mogen niet afhangen van UI-velden of requestbody-invoer.

**Independent Test**: Maak of wijzig reserveringen met een gevalideerde claim en controleer dat de eigenaar uit claims komt; clientinvoer kan geen andere eigenaar afdwingen.

**Acceptance Scenarios**:

1. **Given** een ingelogde medewerker maakt een reservering, **When** de API de aanvraag verwerkt, **Then** wordt de eigenaar afgeleid uit de gevalideerde claims.
2. **Given** een medewerker probeert een reservering van een andere eigenaar te lezen, wijzigen of annuleren, **When** die medewerker geen Administrator role heeft, **Then** wordt de actie geweigerd of niet gevonden zonder eigenaargegevens te lekken.

### User Story 3 - Beheerder autoriseren met Administrator role (Priority: P1)

Als beheerder wil ik beheeracties kunnen uitvoeren op basis van een expliciet toegewezen rol, zodat beheer niet door UI-zichtbaarheid of lokale headers wordt bepaald.

**Why this priority**: Ruimtebeheer en beheer van reserveringen van anderen vereisen een eenduidige server-side autorisatiegrens.

**Independent Test**: Roep beheerendpoints aan met en zonder Administrator role en controleer dat alleen de Administrator toegang krijgt.

**Acceptance Scenarios**:

1. **Given** een ingelogde gebruiker heeft role of app role `Administrator`, **When** die gebruiker een beheerendpoint aanroept, **Then** staat de API de beheeractie toe volgens de bestaande beheerregels.
2. **Given** een ingelogde gebruiker heeft geen Administrator role, **When** die gebruiker een beheerendpoint aanroept, **Then** retourneert de API toegang geweigerd en voert geen wijziging uit.

### User Story 4 - Entra ID-ready zijn voor Azure dev/prod (Priority: P2)

Als technisch team wil ik SmartSpace kunnen configureren voor Entra ID zonder secrets of tenantwaarden in source control, zodat een gedeelde Azure dev-omgeving veilig kan worden ingericht.

**Why this priority**: Echte tenant- en app registration-gegevens zijn omgevingseigendom en mogen niet door de applicatie worden verzonnen of ingecheckt.

**Independent Test**: Configureer de API in Entra ID mode zonder verplichte waarden en controleer dat startup faalt met exacte ontbrekende configuratie; documenteer welke Azure/Entra inrichting nodig is.

**Acceptance Scenarios**:

1. **Given** Entra ID mode is gekozen zonder tenant, client of audience configuratie, **When** de API start, **Then** faalt startup met een duidelijke melding over ontbrekende Entra ID configuratie.
2. **Given** Entra ID mode is gekozen met geldige configuratiewaarden uit de omgeving, **When** een bearer token van de juiste issuer en audience binnenkomt, **Then** valideert de API het token en gebruikt subject- en role-claims voor eigenaarschap en beheer.
3. **Given** de featuredocumentatie wordt gebruikt voor Azure dev, **When** een beheerder de omgeving inricht, **Then** staan app registrations, redirect URI's, API scope, app role en configuratienamen expliciet beschreven zonder echte secrets.

### Edge Cases

- Development identity mag niet impliciet actief worden doordat Entra-configuratie ontbreekt buiten Development.
- Een Entra token zonder herkenbare subjectclaim (`oid` of `sub`) mag niet als eigenaar kunnen boeken.
- Een token zonder role claim geldt als normale Employee, niet als beheerder.
- De UI mag beheerlinks verbergen of tonen, maar API-autorisatie blijft de beslissende grens.
- Ontbrekende tenant/client/audience waarden moeten als configuratiefout worden gemeld, niet als runtime 500 tijdens het eerste request.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Het systeem MUST development-only identity alleen toestaan wanneer de API in Development draait.
- **FR-002**: Het systeem MUST startup laten falen wanneer development identity buiten Development actief is.
- **FR-003**: Het systeem MUST reserveringseigenaarschap server-side afleiden uit gevalideerde claims en nooit uit vrije clientinvoer.
- **FR-004**: Het systeem MUST iedere gevalideerde gebruiker zonder Administrator role als Employee behandelen voor autorisatiebeslissingen.
- **FR-005**: Het systeem MUST beheeracties alleen toestaan aan gebruikers met role of app role `Administrator`.
- **FR-006**: Het systeem MUST een expliciete configuratiemodus ondersteunen voor lokale DevelopmentIdentity en Entra ID bearer token-validatie.
- **FR-007**: Het systeem MUST Entra ID tenant-, client- en audienceconfiguratie uit omgeving/configuratie lezen en geen echte tenant/client-id waarden of secrets hardcoden.
- **FR-008**: Het systeem MUST Entra subjectclaims mappen naar dezelfde server-side eigenaaridentiteit die reserveringscode gebruikt.
- **FR-009**: Het systeem MUST documenteren welke Entra app registrations, API scope, redirect URI's, app role en configuratienamen nodig zijn voor Azure dev/prod.

### Key Entities *(include if feature involves data)*

- **Authenticated Subject**: De gevalideerde gebruiker zoals de API die uit claims afleidt; bevat een stabiele subject-id en optionele role claims.
- **Authentication Mode**: Omgevingsconfiguratie die bepaalt of lokale DevelopmentIdentity of Entra ID bearer token-validatie actief is.
- **Administrator Role**: De role/app role `Administrator` die beheeracties toestaat.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Lokale Development startup blijft werken zonder Entra tenantconfiguratie.
- **SC-002**: Een test bewijst dat development identity buiten Development wordt geweigerd.
- **SC-003**: Een test bewijst dat Administrator role autorisatie bepaalt voor beheerrechten.
- **SC-004**: Een configuratietest of startup-check bewijst dat Entra ID mode ontbrekende verplichte waarden meldt voordat requests worden verwerkt.
- **SC-005**: De documentatie noemt alle Azure/Entra stappen die nodig zijn voor gedeelde dev/prod zonder echte secrets of tenantwaarden.

## Assumptions

- SmartSpace bouwt geen eigen gebruikersdatabase en beheert geen wachtwoorden.
- Medewerker is geen aparte app role; iedere gevalideerde gebruiker geldt als Employee tenzij Administrator role aanwezig is.
- De eerste basis levert API-authenticatie en autorisatieconfiguratie; volledige Blazor/MSAL interactieve login kan een vervolgslice zijn zodra Entra app registrations bestaan.
- Lokale development mag de bestaande demo headers blijven gebruiken zolang dit strikt tot Development beperkt blijft.
