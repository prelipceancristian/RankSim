using RankSim.Core.Matchmaking;
using RankSim.Core.Matchmaking.Scoring;
using RankSim.Core.Matchmaking.Strategies;
using RankSim.Core.Models;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Tests.Matchmaking;

public class BalancedMatchmakerTests
{
    public class TryCreateMatch
    {
        private static readonly FlatRatingStrategy Rating = new();

        private static Player MakePlayer(double rating) => new()
        {
            Id = Guid.NewGuid(),
            Name = $"P{rating}",
            HiddenRating = rating,
            RatingState = Rating.CreateInitialState(rating)
        };

        private static List<WeightedMetric> DefaultMetrics() =>
        [
            new WeightedMetric(new TeamMmrDeltaMetric(), 1.0),
            new WeightedMetric(new IntraTeamSpreadMetric(), 1.0)
        ];

        [Fact]
        public void PoolTooSmallReturnsNull()
        {
            var sut = new BalancedMatchmaker(2, DefaultMetrics(), new Random(42));
            var pool = new List<Player> { MakePlayer(1000), MakePlayer(1100), MakePlayer(1200) };

            Assert.Null(sut.TryCreateMatch(pool));
        }

        [Fact]
        public void ExactPoolSizeCreatesMatch()
        {
            var sut = new BalancedMatchmaker(2, DefaultMetrics(), new Random(42));
            var pool = new List<Player>
            {
                MakePlayer(1000), MakePlayer(1100),
                MakePlayer(1050), MakePlayer(1150)
            };

            var match = sut.TryCreateMatch(pool);

            Assert.NotNull(match);
            Assert.Equal(2, match.TeamA.Count);
            Assert.Equal(2, match.TeamB.Count);
        }

        [Fact]
        public void AllPlayersInMatchAreFromPool()
        {
            var sut = new BalancedMatchmaker(2, DefaultMetrics(), new Random(42));
            var pool = new List<Player>
            {
                MakePlayer(1000), MakePlayer(1100),
                MakePlayer(1050), MakePlayer(1150),
                MakePlayer(900), MakePlayer(1200)
            };

            var match = sut.TryCreateMatch(pool);

            Assert.NotNull(match);
            var allMatched = match.TeamA.Concat(match.TeamB).ToList();
            Assert.All(allMatched, p => Assert.Contains(p, pool));
        }

        [Fact]
        public void NoPlayerAppearsOnBothTeams()
        {
            var sut = new BalancedMatchmaker(2, DefaultMetrics(), new Random(42));
            var pool = new List<Player>
            {
                MakePlayer(1000), MakePlayer(1100),
                MakePlayer(1050), MakePlayer(1150)
            };

            var match = sut.TryCreateMatch(pool);

            Assert.NotNull(match);
            Assert.Empty(match.TeamA.Intersect(match.TeamB));
        }

        [Fact]
        public void SelectsOptimalSplitWithKnownPlayers()
        {
            // With teamSize=2, 4 players: ratings 1000, 1010, 1100, 1110
            // Optimal split: {1000,1110} vs {1010,1100} => team deltas = 0, spreads = 110 and 90
            // Alt splits: {1000,1100} vs {1010,1110} => delta = 0, spreads = 100 and 100
            // The best split minimizes delta + avg spread.
            // {1000,1100} vs {1010,1110} => delta=0, avg_spread=100 => score=100
            // {1000,1110} vs {1010,1100} => delta=0, avg_spread=100 => score=100
            // {1000,1010} vs {1100,1110} => delta=100, avg_spread=10 => score=110
            // So both first splits tie at 100 — either is acceptable.
            var players = new List<Player>
            {
                MakePlayer(1000), MakePlayer(1010),
                MakePlayer(1100), MakePlayer(1110)
            };

            // Use a seed that picks an anchor within this group
            var sut = new BalancedMatchmaker(2, DefaultMetrics(), new Random(42));
            var match = sut.TryCreateMatch(players);

            Assert.NotNull(match);
            var avgA = match.TeamA.Average(p => p.PublicRating);
            var avgB = match.TeamB.Average(p => p.PublicRating);

            // The optimal split should have team avg delta of 0
            Assert.Equal(0, Math.Abs(avgA - avgB), precision: 1);
        }

        [Fact]
        public void AllCandidatesRejectedReturnsNull()
        {
            // Three players at 1000 and one at 2000 — no split can have delta <= 0.01
            var metrics = new List<WeightedMetric>
            {
                new(new TeamMmrDeltaMetric(maxDelta: 0.01), 1.0)
            };
            var sut = new BalancedMatchmaker(2, metrics, new Random(42));
            var pool = new List<Player>
            {
                MakePlayer(1000), MakePlayer(1000),
                MakePlayer(1000), MakePlayer(2000)
            };

            Assert.Null(sut.TryCreateMatch(pool));
        }

        [Fact]
        public void DeterministicWithSameRandomSeed()
        {
            var pool = new List<Player>
            {
                MakePlayer(1000), MakePlayer(1100),
                MakePlayer(1050), MakePlayer(1150),
                MakePlayer(900), MakePlayer(1200)
            };

            var sut1 = new BalancedMatchmaker(2, DefaultMetrics(), new Random(123));
            var sut2 = new BalancedMatchmaker(2, DefaultMetrics(), new Random(123));

            var match1 = sut1.TryCreateMatch(pool);
            var match2 = sut2.TryCreateMatch(pool);

            Assert.NotNull(match1);
            Assert.NotNull(match2);
            Assert.Equal(
                match1.TeamA.Select(p => p.Id),
                match2.TeamA.Select(p => p.Id));
            Assert.Equal(
                match1.TeamB.Select(p => p.Id),
                match2.TeamB.Select(p => p.Id));
        }
    }
}
