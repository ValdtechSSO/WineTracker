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

The architecture was declared and reviewed before product implementation.
`OWN001` is explicitly `REVIEWED` through a
[durable maintainer attestation](https://github.com/ValdtechSSO/WineTracker/issues/1#issuecomment-5299418695)
bound to the rule digest, subject fingerprint, and reviewed commit. The
repository does not claim mechanical proof of PostgreSQL write ownership.

## Architecture validation

```bash
uvx --from agentic-architecture-kit==0.4.3 aak validate --fail-on-review
```

## Run locally

Requirements:

- .NET SDK 10
- Node.js 24 and npm 11
- Docker with Compose

Start PostgreSQL:

```bash
docker compose up -d
```

In a second terminal, restore the local EF tool and start the API:

```bash
cp src/Hosts/Api/appsettings.Development.example.json \
  src/Hosts/Api/appsettings.Development.json
dotnet tool restore
dotnet run --project src/Hosts/Api/WineTracker.Api.csproj
```

The API applies committed migrations on startup and listens at
`http://localhost:5080` in the default development profile. The generated
`appsettings.Development.json` is local-only and ignored by Git. Never commit
that file; change its connection string locally when required.

In a third terminal, install and start the Angular host:

```bash
npm ci --prefix src/Hosts/Web
npm start --prefix src/Hosts/Web
```

Open `http://localhost:4200`. The Angular development proxy forwards `/api`
requests to the backend.

Stop PostgreSQL without deleting the journal data:

```bash
docker compose down
```

## Product model

A wine is identified by producer, label, vintage (or non-vintage), and type.
Recording the same wine again creates another consumption event rather than
overwriting history. Each event stores its own date, optional rating and notes,
and an explicit `yes`, `no`, or `undecided` reorder choice. The order-again list
uses the most recent choice for each wine.

## Product checks

```bash
./tools/check-development-settings.sh
dotnet restore WineTracker.slnx
dotnet build WineTracker.slnx --no-restore
dotnet test WineTracker.slnx --no-build
npm ci --prefix src/Hosts/Web
npm run check --prefix src/Hosts/Web
```

GitHub Actions runs both product checks and the strict AAK architecture gate on
pull requests and pushes to `main`.
