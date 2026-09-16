# Data model: SmartSpace release 1

## Design boundary

Dit model beschrijft businessentiteiten en invarianten, niet de fysieke SQL Server
schema-instructies. SQL Server is de persistente bron van waarheid; migrations en
provider-specifieke details horen bij implementatietaken.

## Location

Representeert een locatie waar vergaderruimtes staan.

- `id`: stabiele identificatie.
- `name`: verplichte herkenbare naam.
- `building`: optioneel aanvullend label.
- `floor`: optioneel aanvullend label.
- `isActive`: bepaalt of de locatie beschikbaar is voor nieuwe boekingen.

Een locatie wordt niet verwijderd zolang eraan gerelateerde ruimtes of historie
bestaat. Zelfstandig locatiebeheer is niet een release-1 gebruikersflow tenzij een
later besluit dat toevoegt.

## Resource / MeetingRoom

Representeert in release 1 een boekbare vergaderruimte.

- `id`: stabiele identificatie.
- `name`: verplichte naam.
- `resourceType`: in release 1 uitsluitend `MeetingRoom`.
- `capacity`: positief geheel getal.
- `locationId`: verplichte relatie naar `Location`.
- `isActive`: actieve ruimtes kunnen nieuw worden geboekt.
- `version`: waarde voor stale-edit detectie.

Deactiveer een gebruikte ruimte in plaats van haar te verwijderen. De naam-uniciteit
binnen een locatie blijft een labvoorstel uit de stakeholderinput totdat BIDN dit
bevestigt.

## Reservation

Representeert een reservering voor één ruimte en één eigenaar.

- `id`: stabiele identificatie.
- `resourceId`: relatie naar `Resource`.
- `ownerSubjectId`: server-afgeleide identiteit van de eigenaar.
- `start`: tijdstip van het begin.
- `end`: tijdstip van het einde, exclusief.
- `status`: `Active` of `Cancelled`.
- `createdAt`: audit-tijdstip.
- `updatedAt`: laatste wijziging.
- `cancelledAt`: optioneel tijdstip van annulering.
- `version`: waarde voor stale-edit detectie.
- `resourceNameAtBooking`: naam van de ruimte bij het boeken.
- `locationNameAtBooking`: locatie van de ruimte bij het boeken.
- `capacityAtBooking`: capaciteit van de ruimte bij het boeken.

Een reservering is geldig wanneer `end > start`. Actieve reserveringen overlappen
wanneer `existing.start < requested.end` én `existing.end > requested.start`.
Aansluitende intervallen zijn dus toegestaan. Geannuleerde reserveringen blokkeren
geen beschikbaarheid.

Afgelopen is een afgeleide weergave op basis van het einde; het is geen afzonderlijke
status. Annuleren verandert status en behoudt de rij. Hard delete van reservations
of gerefereerde rooms is niet toegestaan.

## Time representation

De gebruiker kiest in Europe/Amsterdam. Niet-bestaande lokale tijden worden geweigerd.
Bij een dubbel voorkomende lokale tijd kiest de gebruiker expliciet de bedoelde
variant. De API-contracten dragen een expliciete offset of instant; de precieze
normalisatie blijft een implementatiedetail maar mag geen stille tijdverschuiving
veroorzaken.

## Relationships

```text
Location 1 ─── * Resource 1 ─── * Reservation
Reservation * ─── 1 Employee (identity subject, external to release-1 domain)
```

## State transitions

```text
Reservation: Active -> Cancelled
Reservation: Active -> Active (approved future update, new version)
Resource: Active -> Inactive (only without active reservations where `start <= now < end`)
Resource: Inactive -> Active (administrator, only when Location is active)
```

Een geannuleerde reservering wordt niet opnieuw actief gemaakt in release 1. Een
afgewezen wijziging laat de oorspronkelijke waarden en status intact.

## Validation invariants

- Alleen een ingelogde medewerker kan namens zichzelf reserveren.
- Alleen de eigenaar kan een eigen toekomstige actieve reservering wijzigen of
  annuleren.
- Een beheerder mag in release 1 reserveringen van anderen beheren volgens de
  bevestigde autorisatie- en privacyregels.
- Alleen actieve ruimtes op actieve locaties verschijnen in nieuwe boekingsresultaten.
- Een reservering blokkeert deactivering als `start <= now < end`; op het exclusieve
  eindtijdstip is zij niet meer lopend.
- Actieve overlappende reserveringen voor dezelfde ruimte zijn verboden.
- Een conflict bij gelijktijdige mutaties mag geen twee overlappende actieve rijen
  opleveren.
- Historie blijft onbeperkt voor de eigenaar raadpleegbaar. Beheerders krijgen
  alleen de minimaal noodzakelijke historische gegevens voor hun beheerhandeling.
