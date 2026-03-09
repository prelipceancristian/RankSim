namespace RankSim.Core.Rating.Strategies;

public sealed class FlatRatingStrategy : IRatingStrategy
{
    private readonly double _delta;

    public FlatRatingStrategy(double delta = 25) => _delta = delta;

    public string Name => "Flat";

    public IRatingState CreateInitialState(double initialRating) => new FlatState(initialRating);

    public (MatchTeam Winners, MatchTeam Losers) UpdateRatings(MatchTeam winners, MatchTeam losers)
    {
        var updatedWinners = winners.States
            .Cast<FlatState>()
            .Select(s => (IRatingState)new FlatState(s.Rating + _delta))
            .ToList();

        var updatedLosers = losers.States
            .Cast<FlatState>()
            .Select(s => (IRatingState)new FlatState(s.Rating - _delta))
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
