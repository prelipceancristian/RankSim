namespace RankSim.Core.Matchmaking.Scoring;

public sealed class TeamMmrDeltaMetric(double? maxDelta = null) : IMatchMetric
{
    public string Name => "TeamMmrDelta";

    public double Evaluate(MatchCandidate candidate)
    {
        var avgA = candidate.TeamA.Average(p => p.PublicRating);
        var avgB = candidate.TeamB.Average(p => p.PublicRating);
        var delta = Math.Abs(avgA - avgB);

        return maxDelta.HasValue && delta > maxDelta.Value
            ? double.PositiveInfinity
            : delta;
    }
}
