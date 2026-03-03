# RankSim

RankSim is a **Ranked Matchmaking Simulator** that analyzes how rating strategies (Elo, Glicko-2, TrueSkill) and external factors (smurfing, double-downs, fading interest) affect matchmaking quality. It runs configurable simulations on the backend and presents real-time metric dashboards on the frontend.

## Tech Stack

- **Backend**: .NET 8 (C#), ASP.NET Core Web API, SignalR, Entity Framework Core + SQLite
- **Frontend**: React 18 + TypeScript, Vite, Tailwind CSS, shadcn/ui, Recharts
- **Testing**: xUnit (backend), Vitest + React Testing Library (frontend)

### Backend

```bash
# Run the API (from solution root)
dotnet run --project src/RankSim.Api

# Run all tests
dotnet test

# Run a specific test project
dotnet test tests/RankSim.Core.Tests
dotnet test tests/RankSim.Api.Tests

# Run a single test
dotnet test --filter "FullyQualifiedName~EloRatingSystemTests.UpdateRatings_WinnerGainsRating"

# Add EF migration
dotnet ef migrations add <MigrationName> --project src/RankSim.Infrastructure --startup-project src/RankSim.Api

# Apply migrations
dotnet ef database update --project src/RankSim.Infrastructure --startup-project src/RankSim.Api
```

### Frontend (`src/RankSim.Web/`)

```bash
npm install
npm run dev        # Dev server (Vite)
npm run build      # Production build
npm run test       # Vitest tests
npm run lint       # ESLint
```

## Architecture

**RankSim.Core has zero external dependencies.** All business logic (rating systems, matchmaking, simulation engine, metrics) lives here. The API and Infrastructure layers depend on Core, never the reverse.

**Single database table with JSON columns.** `SimulationRuns` stores `ConfigJson` and `ResultsJson` as serialized text. No complex relational modeling — this is document storage via EF Core + SQLite.

**Two-phase communication model:** Frontend POSTs config → REST API → gets back `simulationId`. Frontend connects SignalR, calls `StartSimulation(simulationId)` → receives `OnProgress` snapshots every N ticks → `OnCompleted` on finish

## After making changes

Always run dotnet test and npm run lint before considering a task done.
