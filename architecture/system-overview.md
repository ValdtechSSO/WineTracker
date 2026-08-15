# System overview

## Purpose

WineTracker is a single-user application for recording wines and individual
consumption events, reviewing the history, and deciding which wines should be
ordered again.

## Capability boundaries

`WineJournal` is the only current functional module. Wine identity, consumption
history, tasting notes, ratings, and reorder intent share vocabulary, state,
invariants, ownership, and lifecycle. Current evidence does not justify another
module or a shared project.

## Hosts

- `Api` exposes WineJournal behavior through HTTP and composes the module with
  PostgreSQL. It does not own product rules.
- `Web` is an Angular 22 client using Angular Material for interactive UI. It
  adapts user input and API output but does not own product rules.

The separate hosts are justified by distinct .NET and browser/Node runtimes.

## Dependency direction

The API host may depend on the WineJournal module. The module never depends on
either host. PostgreSQL implementation details remain behind a module-owned
port. The web host communicates with the API over HTTP and has no direct data
access.

## Known external systems

PostgreSQL 18 is the only current external runtime dependency. Local development
uses Docker Compose because Docker is available and no local `psql` client is
installed.

## Open architectural questions

- AAK 0.4.3 has no built-in Angular adapter. The declared web host root is
  governed, while TypeScript structure relies on Angular build, lint, and tests
  until a separately versioned adapter is justified and pinned.
- Cloud deployment, authentication, external wine catalogs, image storage,
  inventory, and multi-user ownership are not current requirements.
