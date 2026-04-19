using RankSim.Core.Matchmaking;
using RankSim.Core.Models;

namespace RankSim.Core.Engine;

/// <summary>
/// Determines which team wins a completed match.
/// </summary>
public interface IOutcomeDecider
{
    (IReadOnlyList<Player> Winners, IReadOnlyList<Player> Losers) Decide(Match match);
}
