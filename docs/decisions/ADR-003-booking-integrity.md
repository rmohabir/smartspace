# ADR-003: Boekingsintegriteit onder concurrency

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

Een availability-check gevolgd door een losse save kan gelijktijdige overlappende
boekingen toelaten. Meerdere API-processen mogen de invariant niet omzeilen.

## Besluit

Gebruik halfopen intervallen `[start, end)`, transactionele SQL Server-mutaties en
een database/transactionele invariant die actieve overlap voor dezelfde ruimte
verhindert. Herlees en map databaseconflicten naar een betekenisvolle conflictrespons.

## Alternatieven

- Alleen een voorafgaande read-check.
- Een process-local lock.
- Alleen een applicatieconcurrencytoken.

## Motivatie

De invariant moet buiten één API-process gelden. Het exclusieve eindtijdstip laat
10:00-11:00 en 11:00-12:00 aansluiten.

## Consequenties

Integratietests moeten echte SQL Server-concurrentie gebruiken en zowel responses als
database-invarianten controleren.
