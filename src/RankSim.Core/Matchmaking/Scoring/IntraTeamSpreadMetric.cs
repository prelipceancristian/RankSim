namespace RankSim.Core.Matchmaking.Scoring;

public sealed class IntraTeamSpreadMetric(double? maxSpread = null) : IMatchMetric
{
    public string Name => "IntraTeamSpread";

    public double Evaluate(MatchCandidate candidate)
    {
        var spreadA = Spread(candidate.TeamA);
        var spreadB = Spread(candidate.TeamB);
        var avg = (spreadA + spreadB) / 2.0;

        return maxSpread.HasValue && avg > maxSpread.Value
            ? double.PositiveInfinity
            : avg;
    }

    private static double Spread(IReadOnlyList<Models.Player> team)
    {
        var ratings = team.Select(p => p.PublicRating);
        return ratings.Max() - ratings.Min();
    }
}
