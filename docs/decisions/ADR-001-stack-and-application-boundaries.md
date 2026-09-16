# ADR-001: Stack en applicatiegrenzen

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

De stakeholder-TechStack beschrijft Blazor WebAssembly en Minimal API. De actuele
planinput vraagt React, Next.js App Router, TypeScript en controller Web API. De
constitution benoemt de stakeholderstack als vastgelegd.

## Besluit

Gebruik voor deze planvariant .NET 10 controller Web API en React/Next.js App Router
in één monorepo. Dit besluit is **Voorgesteld** en wordt niet als goedgekeurd
beschouwd zonder constitutionele afstemming.

## Alternatieven

- De constitutionele Blazor/Minimal API-stack volgen.
- De huidige planvariant eerst formeel als constitutionele amendment vastleggen.

## Motivatie

De planvariant volgt de meest recente expliciete technische opdracht, terwijl de
status zichtbaar blijft vanwege de bestaande governance.

## Consequenties

Er zijn twee mogelijke richtingen voor implementatie. `/speckit-implement` mag niet
starten zolang de afwijking niet is bevestigd of teruggedraaid.
