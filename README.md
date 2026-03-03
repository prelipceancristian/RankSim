# RankSim

> Built with [Claude Code](https://claude.ai/code) — an experiment in AI-assisted development.

RankSim is a **Ranked Matchmaking Simulator** that explores how rating algorithms (Elo, Glicko-2, TrueSkill) and player behaviors (smurfing, rage-quitting, fading interest) affect matchmaking quality over time.

Configure a simulation, run it, and watch live metric dashboards update in real time as the system evolves through thousands of matches.

## What it does

RankSim models a player pool going through thousands of matches and tracks how well the matchmaking system performs. You can experiment with:

- **Rating algorithms** — Elo, Glicko-2, or TrueSkill, each with configurable parameters
- **Player behaviors** — smurfing (high-skill players on new accounts), double-down (players grinding after losses), and fading interest (players quitting as they lose)
- **Matchmaking strategies** — strict skill matching vs. queue-time balanced approaches
- **Simulation scale** — number of players, matches per tick, total ticks

## Features

- Multiple rating systems: Elo, Glicko-2, TrueSkill
- Configurable behavioral modifiers: smurfs, rage-quitters, disengaging players
- Real-time progress via SignalR — simulation snapshots pushed to the frontend as they're computed
- Persistent history — every run stored with its full config and results
- Comparison view — load past runs side-by-side to compare parameter sets

## Tech Stack

- **Backend**: .NET 8, ASP.NET Core, SignalR, EF Core + SQLite
- **Frontend**: React 18 + TypeScript, Vite, Tailwind CSS, Recharts

## Getting Started

```bash
# Backend
dotnet run --project src/RankSim.Api

# Frontend
cd src/RankSim.Web && npm install && npm run dev
```

Open `http://localhost:5173`.

## License

MIT
