# ADR-002: SQL Server als persistente bron

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

Historie en multi-process concurrency vereisen duurzame opslag. De eerdere prototype-
TechStack gebruikte SQLite in-memory, maar de constitution vereist SQL Server als
persistente bron. SQL Server Developer is uitsluitend bedoeld voor lokale ontwikkeling
en tests. De Compose-tag blijft een leesbaar startpunt; reproduceerbare teamsessies
leggen daarnaast de gekozen image-digest vast.

## Besluit

Gebruik SQL Server voor durable data en EF Core migrations voor schema-evolutie.

## Alternatieven

- SQLite in-memory voor een lokaal prototype.
- SQLite op schijf voor een beperkte proef.
- Azure SQL als hostingvariant.

## Motivatie

SQL Server maakt persistentie, herstel en concurrency in een gedeelde API-omgeving
expliciet toetsbaar.

## Consequenties

Lokale validatie vereist een geïsoleerde SQL Server. Connection strings en secrets
komen uit environment/user-secrets en nooit uit broncode. De lokaal gecontroleerde
digest voor `mcr.microsoft.com/mssql/server:2022-latest` is:

`mcr.microsoft.com/mssql/server@sha256:ea73825f3d88a23c355ac2f9fdc6bd960fec90171c12c572109b36a558f77bb8`
