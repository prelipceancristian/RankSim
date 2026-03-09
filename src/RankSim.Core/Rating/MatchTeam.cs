namespace RankSim.Core.Rating;

/// <summary>
/// Holds the rating states for one side of a match (winners or losers).
/// </summary>
public sealed class MatchTeam(IReadOnlyList<IRatingState> states)
{
    public IReadOnlyList<IRatingState> States { get; } = states;
}
