# WineTracker

## Purpose

Track wines personally consumed, preserve their tasting history, and make the
decision to order them again explicit.

## Start here

- Run `uvx --from agentic-architecture-kit==0.4.3 aak core` before structural decisions.
- Run `uvx --from agentic-architecture-kit==0.4.3 aak guide bootstrap` for the creation and evolution procedure.
- Read `architecture/system-overview.md`, `domain/global-invariants.md`, and applicable ADRs.
- Read `src/Modules/WineJournal/module.contract.yml` and its local `AGENTS.md` before changing product behavior.
- Treat Angular source analysis as project-specific until an AAK Angular adapter is pinned.

## Authoritative commands

- Backend build: `dotnet build WineTracker.slnx`
- Backend tests: `dotnet test WineTracker.slnx`
- Frontend checks: `npm --prefix src/Hosts/Web run check`
- Development settings: `./tools/check-development-settings.sh`
- Architecture: `uvx --from agentic-architecture-kit==0.4.3 aak validate --fail-on-review`

## Critical rules

- A consumption always belongs to exactly one wine.
- The reorder decision is explicit and is never inferred from a rating.
- PostgreSQL access remains behind the WineJournal module's persistence port.
- Angular Material supplies interactive UI components; do not introduce a parallel component system.
- Every `appsettings.Development.json` is local-only and must remain ignored and untracked.
- Never place connection strings, credentials, or other environment-specific secrets in a tracked appsettings file.
- Do not add speculative modules, projects, abstractions, or empty directories.
- Boundary changes update policy, contracts, decisions, validation, and evidence atomically.

## Map

- `src/Modules/WineJournal/`: wine and consumption behavior and owned data.
- `src/Hosts/Api/`: ASP.NET Core transport and composition host.
- `src/Hosts/Web/`: Angular and Angular Material delivery host.
- `architecture/`, `domain/`: maintained decisions and invariants.
- `.agentic/`: AAK version pin, policies, generated context, and evidence.

## Prohibited operations

- Do not delete or rewrite the local PostgreSQL volume without explicit authorization.
- Do not publish personal wine history to an external service without explicit authorization.
