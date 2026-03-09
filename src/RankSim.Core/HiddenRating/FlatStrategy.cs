namespace RankSim.Core.HiddenRating;

/// <summary>
/// Baseline: skill and interest never change.
/// Useful for isolating the effect of the rating system itself.
/// </summary>
public sealed class FlatStrategy : IHiddenRatingStrategy
{
    public string Name => "Flat";

    public void Apply(PlayerTickContext context) { }
}
