namespace RankSim.Core.HiddenRating;

/// <summary>
/// Defines how a player's hidden rating and game interest evolve each simulation tick.
/// </summary>
public interface IHiddenRatingStrategy
{
    string Name { get; }

    /// <summary>
    /// Applies one tick of skill/interest evolution.
    /// Implementations mutate <paramref name="context"/> in place.
    /// </summary>
    void Apply(PlayerTickContext context);
}
