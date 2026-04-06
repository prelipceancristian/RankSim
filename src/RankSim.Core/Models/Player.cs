using RankSim.Core.Rating;

namespace RankSim.Core.Models;

public class Player
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

    /// <summary>
    /// The player's rating state, owned by the active <see cref="IRatingStrategy"/>.
    /// Must be initialized via <see cref="IRatingStrategy.CreateInitialState"/>.
    /// </summary>
    public required IRatingState RatingState { get; set; }

    /// <summary>
    /// The player's displayed rating, derived from <see cref="RatingState"/>.
    /// Used by the matchmaker to find and pair players.
    /// </summary>
    public double PublicRating => RatingState.PublicRating;

    /// <summary>
    /// The number of tokens that the player can use for doubling down their rank win/loss at the beginning of the match.
    /// </summary>
    public int DoubleDownTokens { get; set; } = 100;

    public override string ToString()
    {
        return $"Player[{Name}] " +
               $"Id={Id}, " +
               $"PublicRating={PublicRating:F2}, " +
               $"HiddenRating={HiddenRating:F2}, " +
               $"Interest={GameInterest:F2}, " +
               $"Matches={MatchesPlayed}, " +
               $"LastTick={LastMatchTick}, " +
               $"Tokens={DoubleDownTokens}";
    }
}
