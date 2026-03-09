namespace RankSim.Core.HiddenRating;

/// <summary>
/// Mutable working state passed to an <see cref="IHiddenRatingStrategy"/> each simulation tick.
/// The strategy reads from and writes back to this object; the simulation engine then
/// persists the updated values onto the player.
/// </summary>
public class PlayerTickContext
{
    /// <summary>The player's current hidden (true skill) rating.</summary>
    public double HiddenRating { get; set; }

    /// <summary>
    /// How engaged the player currently is (0 = no interest, 1 = fully engaged).
    /// Strategies may decrease or increase this over time.
    /// </summary>
    public double GameInterest { get; set; }

    /// <summary>Number of simulation ticks elapsed since the player last played a match.</summary>
    public int TicksSinceLastMatch { get; init; }

    /// <summary>Total matches the player has played so far.</summary>
    public int MatchesPlayed { get; init; }
}
