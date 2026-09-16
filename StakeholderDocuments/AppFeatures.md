# SmartSpace eerste release

> Status: stakeholderinput voor de training. Dit document is nog geen
> goedgekeurde productspecificatie. Regels onder "Vaststaande businessregels"
> zijn de oorspronkelijke scope- en businessinput; labvoorstellen staan apart
> en moeten door BIDN worden bevestigd.
 
## Medewerker
- Ruimtes bekijken met naam, capaciteit en locatie.
- Vrije ruimtes zoeken voor een gekozen begin- en eindtijd.
- Een ruimte reserveren voor zichzelf.
- Een eigen toekomstige actieve reservering wijzigen.
- Een eigen toekomstige actieve reservering annuleren.
- Eigen toekomstige, afgelopen en geannuleerde reserveringen bekijken.
 
## Beheerder
- Ruimte toevoegen.
- Naam, positieve capaciteit en locatie aanpassen.
- Ruimte deactiveren wanneer geen actieve lopende of toekomstige
  reserveringen meer bestaan.
- Geen extra bevoegdheden over boekingen van andere medewerkers.
 
## Vaststaande businessregels
- Een reservering heeft een begin- en eindtijd; einde ligt na begin.
- Een nieuwe of gewijzigde boeking begint in de toekomst.
- Aansluitende boekingen mogen; overlappende actieve boekingen niet.
- Ook twee gelijktijdige aanvragen mogen geen dubbele boeking opleveren.
- Een afgewezen wijziging laat de bestaande reservering intact.
- Annuleren bewaart de historie en geeft het tijdvak vrij.
- Historische reserveringen blijven voor de eigenaar raadpleegbaar.
- Andere medewerkers zien geen persoonsgegevens in beschikbaarheid.
- De lokale UI toont tijden voor Europe/Amsterdam.

## Labvoorstellen en te bevestigen uitwerkingen
- Nieuwe of gewijzigde boekingen moeten in de toekomst beginnen; dit is nog te
  bevestigen als beleid voor lopende of direct aansluitende boekingen.
- Aansluitende boekingen zonder buffer zijn toegestaan; een eventuele buffer
  moet als BIDN-beleid worden vastgesteld.
- Deactiveren wordt geweigerd zolang actieve lopende of toekomstige
  reserveringen bestaan; de precieze beheerprocedure en bevoegdheden zijn nog
  open.
 
## Grenzen
Geen herhaalboekingen, notificaties, deelnemerslijsten of integraties.
Geen werkplekfunctionaliteit in release 1.
Overige open besluiten staan in [TechStack.md](TechStack.md), §15, en moeten
worden bevestigd. Dit voorkomt een verwijzing naar het ontbrekende bestand
`OpenQuestions.md`.
