using RankSim.Core.Matchmaking.Scoring;
using RankSim.Core.Models;

namespace RankSim.Core.Matchmaking.Strategies;

public sealed class BalancedMatchmaker(int teamSize, IReadOnlyList<WeightedMetric> metrics,
    Random random) : IMatchmaker
{
    public string Name => "Balanced";

    public Match? TryCreateMatch(IReadOnlyList<Player> pool)
    {
        var poolSize = 2 * teamSize;
        if (pool.Count < poolSize)
        {
            return null;
        }

        var anchor = pool[random.Next(pool.Count)];

        var candidates = pool
            .OrderBy(p => Math.Abs(p.PublicRating - anchor.PublicRating))
            .Take(poolSize)
            .ToList();

        var bestScore = double.PositiveInfinity;
        Match? bestMatch = null;

        foreach (var teamAIndices in CombinationHelper.GetHalfCombinations(poolSize, teamSize))
        {
            var teamASet = new HashSet<int>(teamAIndices);
            var teamA = new List<Player>(teamSize);
            var teamB = new List<Player>(teamSize);

            for (var i = 0; i < poolSize; i++)
            {
                if (teamASet.Contains(i))
                    teamA.Add(candidates[i]);
                else
                    teamB.Add(candidates[i]);
            }

            var candidate = new MatchCandidate(teamA, teamB);
            var score = ScoreCandidate(candidate);

            if (score < bestScore)
            {
                bestScore = score;
                bestMatch = new Match(teamA, teamB);
            }
        }

        return bestMatch;
    }

    private double ScoreCandidate(MatchCandidate candidate)
    {
        var total = 0.0;
        foreach (var wm in metrics)
        {
            var value = wm.Metric.Evaluate(candidate);
            if (double.IsPositiveInfinity(value))
                return double.PositiveInfinity;
            total += value * wm.Weight;
        }
        return total;
    }
}
