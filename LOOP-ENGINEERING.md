# Loop Engineering voor SmartSpace

## Doel

Deze aanpak laat een AI-agent zelfstandig itereren aan een feature, terwijl de
ontwikkelaar het doel, de feedbacklus en de stopvoorwaarden ontwerpt. De agent
mag uitvoeren, controleren en bijsturen, maar mag het doel of de kwaliteitsgrens
niet zelf versoepelen.

De lus is gebaseerd op de bestaande Speckit-documenten en commando's:

```text
spec.md -> plan.md -> tasks.md -> implement -> validate -> converge
```

## Ontwikkelaarscontract

Voor iedere feature legt de ontwikkelaar vooraf vast:

- het doel en de betrokken requirement-ID's;
- de scope: welke bestanden, projecten en tests mogen wijzigen;
- de validatiecommando's en verwachte uitkomst;
- de maximale hoeveelheid iteraties;
- de stopvoorwaarden voor succes, blokkade en menselijke beoordeling.

Voor SmartSpace zijn `spec.md`, `plan.md`, `tasks.md` en de constitution de bron
van waarheid. Een agent mag geen ontbrekende requirement zelf invullen als dat
de scope of het productgedrag verandert.

## Gecontroleerde feedbacklus

Elke ronde heeft dezelfde stappen:

1. **Observeer**: lees de actuele taskstatus, code, testresultaten en logs.
2. **Beoordeel**: vergelijk de waargenomen toestand met requirements, acceptance
   scenarios, planbesluiten en constitutionele regels.
3. **Kies één correctie**: pak de hoogste openstaande blocker of een kleine groep
   logisch samenhangende taken.
4. **Voer uit**: wijzig alleen bestanden binnen de vooraf bepaalde scope.
5. **Valideer**: voer de goedkoopste relevante test of build uit en daarna de
   bredere gate die voor de feature is afgesproken.
6. **Registreer**: leg resultaat, bewijs, resterende afwijking en volgende actie
   vast.
7. **Stop of herhaal**: herhaal alleen als de stopvoorwaarden dat toestaan.

```text
┌──────────┐    ┌──────────┐    ┌──────────┐
│ Observe  │ -> │ Beoordeel│ -> │ Correctie│
└──────────┘    └──────────┘    └────┬─────┘
      ^                              v
      │                         ┌──────────┐
      └────── registreer <──────│ Valideer │
                                └──────────┘
```

## Speckit-gates per ronde

| Fase | Agentactie | Gate | Resultaat |
|---|---|---|---|
| Voorbereiding | `speckit-specify` en eventueel `speckit-clarify` | requirements zijn begrijpelijk en toetsbaar | bijgewerkte `spec.md` |
| Ontwerp | `speckit-plan` | stack, data, contracten en validatie zijn consistent | bijgewerkt ontwerp |
| Decompositie | `speckit-tasks` | elke requirement heeft uitvoerbare taken en tests | bijgewerkte `tasks.md` |
| Pre-implementatie | `speckit-analyze` | geen kritieke inconsistenties | implementatie mag starten |
| Uitvoering | `speckit-implement` | taken worden uitgevoerd en gemarkeerd | code en tests gewijzigd |
| Post-implementatie | build en gerichte tests | gedrag werkt voor de gewijzigde slice | testbewijs |
| Convergentie | `speckit-converge` | geen resterende gaps, of nieuwe traceerbare taken | converged of volgende ronde |

## Stopvoorwaarden

De agent stopt met status **CONVERGED** wanneer alle volgende voorwaarden waar zijn:

- alle relevante tasks in `tasks.md` zijn voltooid;
- de gerichte tests, build en eventuele browserflow slagen;
- `speckit-analyze` geen kritieke inconsistentie meldt;
- `speckit-converge` geen nieuwe taken toevoegt;
- er geen wijziging buiten de afgesproken scope is gemaakt.

De agent stopt met status **BLOCKED** wanneer:

- een test of build na de toegestane herstelpogingen blijft falen;
- een requirement tegenstrijdig of onvoldoende gespecificeerd is;
- een benodigde externe dienst, secret, database of menselijke beslissing ontbreekt;
- een wijziging een constitutioneel principe of beveiligingsgrens zou schenden.

De agent stopt met status **REVIEW_REQUIRED** wanneer:

- productgedrag moet worden gekozen uit meerdere redelijke opties;
- een breaking API-, database- of migratiewijziging nodig is;
- de agent de scope wil uitbreiden;
- dezelfde fout in twee opeenvolgende rondes terugkomt.

## Iteratiebudget en escalatie

Gebruik standaard maximaal drie uitvoeringsrondes per wijziging:

```text
maxRounds = 3
maxRepairAttemptsPerFailure = 2
```

Na iedere ronde wordt het budget verminderd. Een nieuwe ronde is alleen toegestaan
als de vorige ronde meetbare vooruitgang heeft opgeleverd, bijvoorbeeld minder
open taken, een opgeloste testfout of een gesloten requirement-gap.

Bij geen vooruitgang, dezelfde fout, scope-uitbreiding of onduidelijke productkeuze
wordt niet eindeloos opnieuw geprobeerd. De agent schrijft een korte blokkade met:

- de laatste uitgevoerde actie;
- het bewijs van de mislukking;
- de vermoedelijke oorzaak;
- de kleinste beslissing of actie die van de ontwikkelaar nodig is.

## Feedbackrecord

Registreer iedere ronde in een tijdelijk werkdocument, issue of agentresultaat met
minimaal deze velden:

```text
Round: 1
Goal: FR-002 beschikbaarheid filteren op locatie en capaciteit
Scope: src/SmartSpace.Api/Features/Rooms, tests/.../Rooms
Action: availability query aangepast
Validation: dotnet test ... --filter Availability
Result: PASS (8 tests)
Remaining: browserflow nog niet uitgevoerd
Next: run browser validation
Decision: continue
```

Het record is bewijs voor de beslissing om door te gaan. Een groen resultaat mag
niet worden geclaimd zonder een uitvoerbaar commando, testresultaat of expliciete
reden waarom die validatie niet beschikbaar is.

## Veilige agentgrenzen

- Geen `git reset --hard`, force-push, branchverwijdering of destructieve databaseactie.
- Geen secrets lezen, tonen of in logs schrijven.
- Geen requirements, acceptance scenarios of tests verwijderen om een gate groen te maken.
- Geen bestaande gebruikersdata of migraties verwijderen zonder expliciete beslissing.
- Geen parallelle wijzigingen aan hetzelfde bestand zonder coördinatie.
- Een agent commit alleen wanneer dat expliciet is opgedragen.
- Bij onzekerheid: pauzeren met `REVIEW_REQUIRED`, niet gokken.

## Praktische commandoloop

Voor een bestaande featurewijziging:

```text
/speckit-specify <wijziging>
/speckit-clarify                 # indien nodig
/speckit-plan
/speckit-tasks
/speckit-analyze
/speckit-implement
<gerichte build/test-validatie>
/speckit-converge
```

Wanneer `speckit-converge` nieuwe taken toevoegt, start een nieuwe ronde bij:

```text
/speckit-implement
```

Voer daarna opnieuw de gerichte validatie en `/speckit-converge` uit. Herhaal dit
alleen binnen het vooraf vastgelegde iteratiebudget.

## Relatie met de hoofdworkflow

De algemene documentvolgorde staat in [SPECKIT-WORKFLOW.md](SPECKIT-WORKFLOW.md).
Dit document voegt daar de uitvoeringslus, kwaliteitsgates, stopvoorwaarden en
agentgrenzen aan toe.
