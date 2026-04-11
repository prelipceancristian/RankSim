namespace RankSim.Core.Matchmaking.Scoring;

public interface IMatchMetric
{
    string Name { get; }

    /// <summary>
    /// Computes a raw score for this metric. Lower is better.
    /// Return <see cref="double.PositiveInfinity"/> to reject the candidate.
    /// </summary>
    double Evaluate(MatchCandidate candidate);
}
