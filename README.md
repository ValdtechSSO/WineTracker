# WineTracker

WineTracker is a public example of bootstrapping a new application with
[Agentic Architecture Kit](https://github.com/ValdtechSSO/AgenticArchitectureKit).
It records wines and individual consumption events so a user can review what
they drank and decide what to order again.

## Current product scope

- Record a wine and a consumption event.
- Browse consumption history.
- Mark reorder intent as yes, no, or undecided.
- Keep multiple consumption events for the same wine.

Authentication, cloud deployment, external wine catalogs, inventory, image
storage, and automated recommendations are intentionally outside the current
scope.

## Architecture

- .NET 10 LTS backend.
- Angular 22 web client using Angular Material for interactive components.
- PostgreSQL 18, run locally with Docker Compose.
- One functional module: `WineJournal`.
- Two delivery hosts: ASP.NET Core `Api` and Angular `Web`.
- Agentic Architecture Kit 0.4.3 in `solo-maintainer` mode.

Read `AGENTS.md`, `architecture/system-overview.md`, and
`architecture/decisions/ADR-001-initial-architecture.md` before changing the
structure.

## Bootstrap status

The architecture is declared before product implementation. `OWN001` remains a
visible semantic review until the sole maintainer posts a durable GitHub
attestation and the fingerprint-bound review is committed. This is intentional:
the repository does not claim mechanical proof of PostgreSQL write ownership.

## Architecture validation

```bash
uvx --from agentic-architecture-kit==0.4.3 aak validate --fail-on-review
```

Product build and run commands will be added when the first vertical slice is
materialized.
