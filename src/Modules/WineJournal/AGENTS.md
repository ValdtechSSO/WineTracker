# WineJournal module

## Purpose

Own wine identity, consumption history, tasting observations, and explicit
reorder intent.

## Read before changing

- `module.contract.yml`
- `domain/global-invariants.md`
- `architecture/decisions/ADR-001-initial-architecture.md`

## Commands

- `dotnet test WineTracker.slnx`
- `uvx --from agentic-architecture-kit==0.4.3 aak validate --fail-on-review`

## Critical rules

- A consumption references one wine and never replaces an earlier consumption.
- Reorder intent is explicit and independent from rating.
- Keep orchestration with the cohesive WineJournal behavior.
- Keep PostgreSQL and external integrations behind module-owned ports.
