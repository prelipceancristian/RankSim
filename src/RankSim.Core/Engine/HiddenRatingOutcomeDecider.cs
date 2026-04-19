using RankSim.Core.Matchmaking;
using RankSim.Core.Models;

namespace RankSim.Core.Engine;

/// <summary>
/// Decides the match outcome by computing each team's win probability from their
/// average hidden ratings, then sampling a random result against that probability.
/// </summary>
public sealed class HiddenRatingOutcomeDecider(Random random) : IOutcomeDecider
{
    public (IReadOnlyList<Player> Winners, IReadOnlyList<Player> Losers) Decide(Match match)
    {
        var avgHiddenA = match.TeamA.Average(p => p.HiddenRating);
        var avgHiddenB = match.TeamB.Average(p => p.HiddenRating);
        var total = avgHiddenA + avgHiddenB;
        var winProbA = total < double.Epsilon ? 0.5 : avgHiddenA / total;

        var teamAWins = random.NextDouble() < winProbA;
        return teamAWins
            ? (match.TeamA, match.TeamB)
            : (match.TeamB, match.TeamA);
    }
}
