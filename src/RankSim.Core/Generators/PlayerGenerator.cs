using Bogus;
using RankSim.Core.Models;
using RankSim.Core.Rating;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Generators;

public class PlayerGenerator(Random randomGenerator)
{
    private const int MinRating = 1;
    private const int MaxRating = 5000;
    private readonly Faker _faker = new();
#pragma warning disable CA1859
    private readonly IDistribution<double> _hiddenRankDistribution
        = new NormalDistribution((MinRating + MaxRating) / 2, (MinRating + MaxRating) / 6);
    private readonly IRatingStrategy _ratingStrategy = new FlatRatingStrategy();
#pragma warning restore CA1859

    public Player Generate()
    {
        var hiddenRanking = _hiddenRankDistribution.Sample(randomGenerator);
        hiddenRanking = Math.Clamp(hiddenRanking, 1, 5000);
        var state = _ratingStrategy.CreateInitialState(hiddenRanking);
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Name = _faker.Internet.UserName(),
            HiddenRating = hiddenRanking,
            RatingState = state,
        };

        return player;
    }
}
