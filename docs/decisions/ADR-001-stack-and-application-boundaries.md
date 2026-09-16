# ADR-001: Stack en applicatiegrenzen

- Status: **Voorgesteld**
- Datum: 2026-09-16

## Context

De stakeholder-TechStack en constitution beschrijven Blazor WebAssembly en ASP.NET
Core Minimal API als de vastgelegde stack.

## Besluit

Gebruik .NET 10, ASP.NET Core Minimal API en standalone Blazor WebAssembly in één
monorepo. Dit besluit volgt de constitutionele stack.

## Alternatieven

- Een controllergebaseerde API met een alternatieve frontend gebruiken.
- Een aparte frontend/API-stack als constitutionele amendment vastleggen.

## Motivatie

De gekozen stack volgt de bestaande constitution en stakeholderinput. Een eerdere
afwijkende frontend/API-keuze is verwijderd.

## Consequenties

Er is één constitutioneel consistente implementatierichting. `/speckit-implement`
mag de stack volgen zodra de overige open productbesluiten zijn gesloten of als
expliciete blokkades zijn vastgelegd.
