namespace RankSim.Core.Models;

public abstract class Player
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    /// <summary>
    /// The player's true skill, used internally to resolve match outcomes.
    /// Not visible to players — represents ground truth in simulation.
    /// </summary>
    public double HiddenRating { get; set; }

    /// <summary>
    /// How engaged the player currently is (0 = no interest, 1 = fully engaged).
    /// Evolves each tick via the chosen <see cref="HiddenRating.IHiddenRatingStrategy"/>.
    /// </summary>
    public double GameInterest { get; set; } = 1.0;

    /// <summary>Total matches played so far in the simulation.</summary>
    public int MatchesPlayed { get; set; }

    /// <summary>Simulation tick on which the player last played a match.</summary>
    public int LastMatchTick { get; set; }
}
