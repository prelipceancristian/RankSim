namespace RankSim.Core.Matchmaking.Scoring;

public sealed class WeightedMetric(IMatchMetric metric, double weight)
{
    public IMatchMetric Metric { get; } = metric;
    public double Weight { get; } = weight;
}
