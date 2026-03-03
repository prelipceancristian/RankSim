# RankSim

> **An experiment built with [Claude Code](https://claude.ai/code)** — this project was designed and implemented through an AI-assisted development workflow using Anthropic's Claude Code CLI.

RankSim is a **Ranked Matchmaking Simulator** that lets you explore how different rating strategies and real-world player behaviors affect matchmaking quality over time. Configure a simulation, run it, and watch live metric dashboards as the system evolves.

## What it does

RankSim models a player pool going through thousands of matches and tracks how well the matchmaking system performs. You can experiment with:

- **Rating algorithms** — Elo, Glicko-2, or TrueSkill, each with configurable parameters
- **Player behaviors** — smurfing (high-skill players on new accounts), double-down (players grinding after losses), and fading interest (players quitting as they lose)
- **Matchmaking strategies** — strict skill matching vs. queue-time balanced approaches
- **Simulation scale** — number of players, matches per tick, total ticks

Metrics tracked include rating convergence, match quality (skill gap between opponents), queue times, and rank distribution over time — all visualized as real-time charts while the simulation runs.

## Features

- **Multiple rating systems**: Elo (classic), Glicko-2 (rating deviation + volatility), TrueSkill (Bayesian, designed for team games)
- **Behavioral modifiers**: configurable rates for smurfs, rage-quitters, and disengaging players
- **Real-time progress**: SignalR pushes simulation snapshots to the frontend as they're computed
- **Persistent history**: every simulation run is stored with its full config and results
- **Comparison view**: load past runs side-by-side to compare parameter sets
- **Fully tested**: xUnit for backend logic, Vitest + React Testing Library for frontend components

## Tech stack

| Layer | Technology |
|---|---|
| Backend API | .NET 8 / ASP.NET Core / SignalR |
| Rating engine | C# (zero external deps, pure domain logic) |
| Persistence | Entity Framework Core + SQLite |
| Frontend | React 18 + TypeScript + Vite |
| UI components | Tailwind CSS + shadcn/ui |
| Charts | Recharts |
| Backend tests | xUnit |
| Frontend tests | Vitest + React Testing Library |

## Project structure

```
RankSim/
├── src/
│   ├── RankSim.Core/          # Rating systems, matchmaking, simulation engine (no external deps)
│   ├── RankSim.Api/           # ASP.NET Core Web API + SignalR hub
│   ├── RankSim.Infrastructure/ # EF Core + SQLite persistence
│   └── RankSim.Web/           # React + TypeScript frontend
└── tests/
    ├── RankSim.Core.Tests/    # Unit tests for core domain logic
    └── RankSim.Api.Tests/     # Integration tests for API endpoints
```

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)

### Run the backend

```bash
dotnet run --project src/RankSim.Api
```

### Run the frontend

```bash
cd src/RankSim.Web
npm install
npm run dev
```

Open `http://localhost:5173` — the frontend proxies API calls to the .NET backend.

### Run tests

```bash
# Backend
dotnet test

# Frontend
cd src/RankSim.Web && npm run test
```

## Architecture notes

**RankSim.Core has zero external dependencies.** All business logic (rating systems, matchmaking, simulation engine, metrics) lives here. The API and Infrastructure layers depend on Core; Core depends on nothing.

**Single-table persistence.** `SimulationRuns` stores `ConfigJson` and `ResultsJson` as serialized text columns — document storage on top of SQLite rather than relational modeling.

**Two-phase communication.** Frontend POSTs a config to the REST API and receives a `simulationId`. It then connects via SignalR and calls `StartSimulation(simulationId)`, receiving `OnProgress` snapshots every N ticks until `OnCompleted` fires.

## License

MIT
