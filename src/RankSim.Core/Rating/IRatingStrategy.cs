namespace RankSim.Core.Rating;

public interface IRatingStrategy
{
    string Name { get; }

    /// <summary>Creates the initial rating state for a new player.</summary>
    IRatingState CreateInitialState(double initialRating);

    /// <summary>
    /// Updates ratings for all players in both teams after a match.
    /// Returns updated states at the same indices as the inputs.
    /// </summary>
    (MatchTeam Winners, MatchTeam Losers) UpdateRatings(MatchTeam winners, MatchTeam losers);
}
