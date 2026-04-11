using RankSim.Core.Models;

namespace RankSim.Core.Matchmaking;

public sealed class Match(IReadOnlyList<Player> teamA, IReadOnlyList<Player> teamB)
{
    public IReadOnlyList<Player> TeamA { get; } = teamA;
    public IReadOnlyList<Player> TeamB { get; } = teamB;

    public override string ToString() => $"TeamA: {string.Join(", ", TeamA.Select(p => Math.Truncate(p.HiddenRating)))}, TeamB: {string.Join(", ", TeamB.Select(p => Math.Truncate(p.HiddenRating)))}";
}
