# GitHub Copilot Workshop — Quick Start

A 45-minute hands-on workshop kit. Three-tier sample app with **intentional bugs** and **sparse tests** designed for live Copilot demos.

## Files
- [WORKSHOP.md](WORKSHOP.md) — the 45-min instructor script (segment-by-segment, timed)
- [PROMPTS.md](PROMPTS.md) — good vs bad prompts, slash/variable/participant cheat sheet
- [COVERAGE.md](COVERAGE.md) — guided test-coverage exercise (gap analysis → /tests → coverage report)
- [SWAGGER.md](SWAGGER.md) — Swagger/OpenAPI prompt playbook (XML docs, ProducesResponseType, ProblemDetails, client gen)
- [BUGS.md](BUGS.md) — **instructor only** — list of planted bugs & coverage gaps
- [diagrams/architecture.md](diagrams/architecture.md) — Mermaid diagrams for the round-trip exercise
- `backend/` — .NET 8 Web API + EF Core + SQLite
- `backend.Tests/` — xUnit tests (deliberately sparse)
- `frontend/` — React (Vite) SPA
- `database/schema.sql` — schema + seed reference

## Run it

```powershell
# Terminal 1 — API
cd backend
dotnet run
# -> http://localhost:5080  (Swagger at /swagger)

# Terminal 2 — Web
cd frontend
npm install
npm run dev
# -> http://localhost:5173

# Tests
cd backend.Tests
dotnet test

# Tests + coverage report
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
dotnet tool install -g dotnet-reportgenerator-globaltool   # one-time
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

The API auto-creates `shop.db` (SQLite) on first run and seeds two orders (one of which intentionally crashes — see [BUGS.md](BUGS.md)).

## How to use this kit

1. Read [WORKSHOP.md](WORKSHOP.md) end-to-end once.
2. Walk through it yourself with Copilot first — your prompts will be smoother live.
3. Keep [BUGS.md](BUGS.md) open on a second monitor as your cheat sheet.
4. Don't accept Copilot's first answer just because it looks good — **modeling skepticism is half the lesson.**
