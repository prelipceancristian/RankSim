using RankSim.Core.Models;

namespace RankSim.Core.Matchmaking.Scoring;

public sealed class MatchCandidate(IReadOnlyList<Player> teamA, IReadOnlyList<Player> teamB)
{
    public IReadOnlyList<Player> TeamA { get; } = teamA;
    public IReadOnlyList<Player> TeamB { get; } = teamB;
}
