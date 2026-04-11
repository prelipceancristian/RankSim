using RankSim.Core.Matchmaking.Scoring;
using RankSim.Core.Models;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Tests.Matchmaking.Scoring;

public class IntraTeamSpreadMetricTests
{
    public class Evaluate
    {
        private static readonly FlatRatingStrategy Rating = new();

        private static Player MakePlayer(double rating) => new()
        {
            Id = Guid.NewGuid(),
            Name = $"P{rating}",
            HiddenRating = rating,
            RatingState = Rating.CreateInitialState(rating)
        };
        [Fact]
        public void IdenticalPlayersReturnZero()
        {
            var metric = new IntraTeamSpreadMetric();
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1000)],
                [MakePlayer(1000), MakePlayer(1000)]);

            Assert.Equal(0, metric.Evaluate(candidate));
        }

        [Fact]
        public void ReturnsAverageSpread()
        {
            var metric = new IntraTeamSpreadMetric();
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1200)],  // spread = 200
                [MakePlayer(800), MakePlayer(1000)]);   // spread = 200

            Assert.Equal(200, metric.Evaluate(candidate));
        }

        [Fact]
        public void AveragesUnevenSpreads()
        {
            var metric = new IntraTeamSpreadMetric();
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1400)],  // spread = 400
                [MakePlayer(900), MakePlayer(1000)]);   // spread = 100

            Assert.Equal(250, metric.Evaluate(candidate));
        }

        [Fact]
        public void ExceedingThresholdReturnsInfinity()
        {
            var metric = new IntraTeamSpreadMetric(maxSpread: 100);
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1300)],
                [MakePlayer(800), MakePlayer(1000)]);

            Assert.Equal(double.PositiveInfinity, metric.Evaluate(candidate));
        }

        [Fact]
        public void WithinThresholdReturnsValue()
        {
            var metric = new IntraTeamSpreadMetric(maxSpread: 500);
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1200)],  // spread = 200
                [MakePlayer(800), MakePlayer(1000)]);   // spread = 200

            Assert.Equal(200, metric.Evaluate(candidate));
        }
    }
}
