namespace RankSim.Core.Rating.Strategies;

public sealed class FlatRatingStrategy(double delta = 25) : IRatingStrategy
{
    public string Name => "Flat";

    public IRatingState CreateInitialState(double initialRating) => new FlatState(initialRating);

    public (MatchTeam Winners, MatchTeam Losers) UpdateRatings(MatchTeam winners, MatchTeam losers)
    {
        var updatedWinners = winners.States
            .Cast<FlatState>()
            .Select(IRatingState (s) => new FlatState(s.Rating + delta))
            .ToList();

        var updatedLosers = losers.States
            .Cast<FlatState>()
            .Select(IRatingState (s) => new FlatState(s.Rating - delta))
            .ToList();

        return (new MatchTeam(updatedWinners), new MatchTeam(updatedLosers));
    }

    private sealed class FlatState : IRatingState
    {
        public double Rating { get; }
        public double PublicRating => Rating;

        internal FlatState(double rating) => Rating = rating;
    }
}
