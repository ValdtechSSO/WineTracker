# ADR-001: Initial WineTracker architecture

Status: accepted

## Context

The product currently has one user and one cohesive capability: record wines
and consumption history so previous choices can inform future orders. The user
selected .NET, Angular with Angular Material, and PostgreSQL. Initial deployment
is local and the repository has one maintainer.

## Decision

Use one `WineJournal` functional module, an ASP.NET Core API host, an Angular 22
web host, and PostgreSQL 18 as module-owned infrastructure. Target .NET 10 LTS.
Run PostgreSQL locally with Docker Compose. Govern the repository with AAK 0.4.3
in `solo-maintainer` mode under `@valdtechsecurity`.

## Consequences

The backend and frontend have separate runtime boundaries, while all product
rules remain in one module. PostgreSQL migrations and persistence are owned by
WineJournal rather than a technical persistence module. Angular source is
checked by project tooling because the pinned AAK version does not include an
Angular adapter. Authentication, cloud deployment, external catalog lookup,
inventory, and recommendation algorithms require new evidence and review.

## Alternatives considered

- A CLI was rejected because a responsive web interface better supports quick
  entry and browsing.
- SQLite was replaced by the user's PostgreSQL decision.
- Multiple domain modules were rejected because current data and behavior share
  one owner and lifecycle.
