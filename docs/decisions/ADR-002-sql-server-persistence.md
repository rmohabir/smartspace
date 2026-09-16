# ADR-002: SQL Server als persistente bron

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

Historie en multi-process concurrency vereisen duurzame opslag. De eerdere prototype-
TechStack gebruikte SQLite in-memory, maar de constitution vereist SQL Server als
persistente bron.

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
komen uit environment/user-secrets en nooit uit broncode.
