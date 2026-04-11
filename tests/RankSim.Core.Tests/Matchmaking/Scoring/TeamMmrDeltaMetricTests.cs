using RankSim.Core.Matchmaking.Scoring;
using RankSim.Core.Models;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Tests.Matchmaking.Scoring;

public class TeamMmrDeltaMetricTests
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
        public void EqualTeamsReturnZero()
        {
            var metric = new TeamMmrDeltaMetric();
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1000)],
                [MakePlayer(1000), MakePlayer(1000)]);

            Assert.Equal(0, metric.Evaluate(candidate));
        }

        [Fact]
        public void ReturnsAbsoluteDelta()
        {
            var metric = new TeamMmrDeltaMetric();
            var candidate = new MatchCandidate(
                [MakePlayer(1000), MakePlayer(1200)],
                [MakePlayer(800), MakePlayer(900)]);

            // avg A = 1100, avg B = 850 => delta = 250
            Assert.Equal(250, metric.Evaluate(candidate));
        }

        [Fact]
        public void ExceedingThresholdReturnsInfinity()
        {
            var metric = new TeamMmrDeltaMetric(maxDelta: 100);
            var candidate = new MatchCandidate(
                [MakePlayer(1000)],
                [MakePlayer(800)]);

            Assert.Equal(double.PositiveInfinity, metric.Evaluate(candidate));
        }

        [Fact]
        public void WithinThresholdReturnsValue()
        {
            var metric = new TeamMmrDeltaMetric(maxDelta: 300);
            var candidate = new MatchCandidate(
                [MakePlayer(1000)],
                [MakePlayer(800)]);

            Assert.Equal(200, metric.Evaluate(candidate));
        }
    }
}
