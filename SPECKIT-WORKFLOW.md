# SmartSpace Speckit Workflow

Dit document beschrijft de volgorde waarin de SmartSpace-specificatie en de implementatie worden uitgewerkt met Speckit.

## Broninformatie

De inhoudelijke input komt uit:

- `StakeholderDocuments/ProjectGoals.md`
- `StakeholderDocuments/AppFeatures.md`
- `StakeholderDocuments/TechStack.md`
- `StakeholderDocuments/StakeholderReview.md`
- `.specify/memory/constitution.md`

Deze documenten zijn input. Het eerste gegenereerde feature-document is `spec.md`.

## Workflow

### 1. Feature-specificatie

```text
/speckit-specify <featurebeschrijving>
```

Voor SmartSpace:

```text
/speckit-specify Specificeer SmartSpace release 1 voor het reserveren van vergaderruimtes binnen BIDN
```

Output:

- `specs/<feature>/spec.md`
- `specs/<feature>/checklists/requirements.md`

`spec.md` bevat de scope, user stories, acceptance scenarios, edge cases,
functional requirements en succescriteria.

### 2. Verduidelijkingen

```text
/speckit-clarify
```

Gebruik dit wanneer `spec.md` nog `[NEEDS CLARIFICATION]`-punten bevat. Het
commando werkt `spec.md` bij. Deze stap hoort voor `/speckit-plan` te worden
afgerond.

### 3. Technisch plan en ontwerp

```text
/speckit-plan
```

Output:

- `plan.md`
- `research.md`
- `data-model.md`
- `contracts/openapi.yaml` of andere contractbestanden
- `quickstart.md`

In SmartSpace beschrijven deze documenten respectievelijk de technische stack,
onderzoeksbesluiten, domeinmodellen, API-contracten en validatiescenario's.

### 4. Implementatietaken

```text
/speckit-tasks
```

Output:

- `tasks.md`

`tasks.md` zet de requirements om naar genummerde, afhankelijkheidsbewuste
taken, bijvoorbeeld `T001`, `T002` en taken per user story.

### 5. Consistentiecontrole

```text
/speckit-analyze
```

Deze controle vergelijkt `spec.md`, `plan.md` en `tasks.md` op ontbrekende
requirements, tegenstrijdigheden, onduidelijke keuzes en ontbrekende tests.

### 6. Implementatie

```text
/speckit-implement
```

Voert de taken uit `tasks.md` uit en bouwt de broncode, API, UI, databasewijzigingen
en tests volgens het goedgekeurde plan.

### 7. Convergentiecontrole

```text
/speckit-converge
```

Controleert na implementatie welke onderdelen uit de specificatie nog ontbreken.
Eventuele resterende werkzaamheden worden als nieuwe taken aan `tasks.md`
toegevoegd.

## Iteratie: feature wijzigen of toevoegen

Gebruik bij iedere wijziging of uitbreiding dezelfde documentketen. Begin altijd
bij de gewenste gebruikerswaarde en werk daarna de afgeleide documenten bij.

### Bestaande feature wijzigen

1. Beschrijf de gewenste wijziging in een nieuwe of bijgewerkte feature-input.
2. Werk de betreffende user story, scenario's en requirements bij met:

        ```text
        /speckit-specify <beschrijving van de wijziging>
        ```

        Gebruik de bestaande featuremap wanneer de wijziging bij dezelfde feature hoort.
3. Beantwoord nieuwe onduidelijkheden:

        ```text
        /speckit-clarify
        ```

4. Werk het technische ontwerp opnieuw uit:

        ```text
        /speckit-plan
        ```

        Controleer hierbij ook `research.md`, `data-model.md`, contracten en
        `quickstart.md`. Werk ADR's bij als een architectuurbesluit verandert.
5. Genereer de bijgewerkte taken:

        ```text
        /speckit-tasks
        ```

6. Controleer de samenhang voordat je code aanpast:

        ```text
        /speckit-analyze
        ```

7. Voer de nieuwe of gewijzigde taken uit:

        ```text
        /speckit-implement
        ```

8. Controleer welke onderdelen nog ontbreken:

        ```text
        /speckit-converge
        ```

### Nieuwe feature toevoegen

1. Start een nieuwe feature-specificatie; hergebruik niet ongemerkt de
        `spec.md` van een andere feature:

        ```text
        /speckit-specify <beschrijving van de nieuwe feature>
        ```

2. Laat Speckit een nieuwe featuremap aanmaken onder `specs/`.
3. Doorloop, indien nodig, de verduidelijkingsstap:

        ```text
        /speckit-clarify
        ```

4. Maak het plan en de ontwerpdocumenten:

        ```text
        /speckit-plan
        ```

5. Maak de uitvoerbare taken:

        ```text
        /speckit-tasks
        ```

6. Voer de consistentiecontrole uit:

        ```text
        /speckit-analyze
        ```

7. Implementeer de feature in kleine, testbare stappen:

        ```text
        /speckit-implement
        ```

8. Voer de volledigheidscontrole uit:

        ```text
        /speckit-converge
        ```

### Iteratieregels

- Verander eerst de specificatie; verander niet alleen de code.
- Werk requirements, acceptance scenarios, API-contracten, datamodel en tests
  samen bij wanneer de wijziging die raakt.
- Voeg bij een breaking API-wijziging een expliciete contractwijziging en
  migratiepad toe.
- Werk een ADR bij of voeg een ADR toe wanneer een architectuurkeuze verandert.
- Voer na iedere inhoudelijke wijziging ten minste `/speckit-analyze` uit vóór
  `/speckit-implement`.
- Gebruik `/speckit-converge` na implementatie om ontbrekende werkzaamheden terug
  te brengen naar `tasks.md`.

## Documentvolgorde in SmartSpace

```text
StakeholderDocuments/*
        |
        v
specs/001-meeting-room-reservations/spec.md
        |
        v
/speckit-clarify  (optioneel, indien nodig)
        |
        v
specs/001-meeting-room-reservations/plan.md
        |
        +--> research.md
        +--> data-model.md
        +--> contracts/openapi.yaml
        +--> quickstart.md
        |
        v
specs/001-meeting-room-reservations/tasks.md
        |
        v
/speckit-analyze
        |
        v
/speckit-implement
        |
        v
/speckit-converge
```

## Belangrijke architectuurdocumenten

De technische keuzes worden aanvullend vastgelegd in:

- `docs/decisions/ADR-001-stack-and-application-boundaries.md`
- `docs/decisions/ADR-002-sql-server-persistence.md`
- `docs/decisions/ADR-003-booking-integrity.md`
- `docs/decisions/ADR-004-identity-and-roles.md`
- `docs/decisions/ADR-005-timezone-policy.md`
- `docs/decisions/ADR-006-history-and-room-lifecycle.md`

ADR's onderbouwen architectuurkeuzes; ze vervangen `spec.md`, `plan.md` of
`tasks.md` niet.

## Huidige featuremap

De huidige feature staat onder:

```text
specs/001-meeting-room-reservations/
```

De root `README.md` blijft gericht op lokaal starten en bouwen van SmartSpace.
Deze workflowgids beschrijft uitsluitend de Speckit-document- en implementatiefase.

Voor een gecontroleerde AI-feedbacklus met validatie, iteratiebudget,
stopvoorwaarden en agentgrenzen, zie [LOOP-ENGINEERING.md](LOOP-ENGINEERING.md).
