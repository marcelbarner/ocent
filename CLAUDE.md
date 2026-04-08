# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**ocent** is a self-hosted personal operations hub covering finance, documents, storage, and contracts. The codebase is in early/concept stage — backend is a minimal ASP.NET Core shell, frontend is an Angular scaffold.

## Commands

### Local Development

```bash
# Start full Aspire dev environment (launches orchestration dashboard)
dotnet run --project src/DevEnvironment/src/ocent.DevEnvironment.AppHost

# Frontend dev server (standalone)
cd src/frontend && npm install && npm start
```

### Tests

```bash
dotnet test                         # All .NET tests
cd src/frontend && npm test         # Frontend (Vitest)
```

To run a single .NET test: use `--filter "FullyQualifiedName~<TestName>"` with `dotnet test`.

### Linting & Formatting

```bash
dotnet format                        # Format C# code
cd src/frontend && npm run lint      # All frontend linting (TS + SCSS)
cd src/frontend && npm run lint:fix  # Auto-fix all frontend lint issues
npx prettier --write .               # Format frontend files
```

### Mutation Testing

```bash
dotnet stryker --test-runner mtp     # .NET mutation tests
cd src/frontend && npm run mutate    # Frontend mutation tests
```

Quality gates: **80% test coverage**, **80 mutation score** minimum.

## Architecture

```
src/
├── backend/
│   ├── src/ocent.Backend.WebApi/    # ASP.NET Core Web API (minimal, OpenAPI)
│   └── tests/                        # TUnit-based unit tests
├── DevEnvironment/
│   ├── src/ocent.DevEnvironment.AppHost/  # Aspire orchestration host
│   └── tests/                              # TUnit.Aspire acceptance tests
└── frontend/                          # Angular 21, Vite, TypeScript 5.9
    └── src/app/                       # Signal-based components, standalone routing
```

**Key structural decisions:**

- **Aspire** orchestrates the backend for local dev and acceptance testing — `TUnit.Aspire` runs integration tests against the live Aspire environment.
- **Central NuGet management** via `Directory.Packages.props` — add package versions there, not in individual `.csproj` files.
- **Frontend** uses Vite (`@analogjs/vite-plugin-angular`), not the Angular CLI webpack builder.
- **Issue tracking** uses Beads (`.beads/`), not GitHub Issues. Use `bd` CLI commands for all task management.

## .NET Conventions

- SDK version pinned to `10.0.103` via `global.json`.
- Solution file: `ocent.slnx` (new `.slnx` format).
- Tests use **TUnit** (not xUnit/NUnit) — async-first, attribute-based.
- EditorConfig enforces 2-space indent and UTF-8 for all files.

## Frontend Conventions

- Strict TypeScript (`noImplicitOverride`, full strict mode, ES2022 target).
- Components use Angular signals (`signal()`, `computed()`), not RxJS-heavy patterns.
- SCSS for styles; StyleLint enforces formatting.
