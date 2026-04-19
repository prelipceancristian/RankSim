using RankSim.Core.Engine;
using RankSim.Core.Matchmaking;
using RankSim.Core.Models;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Tests.Engine;

public class HiddenRatingOutcomeDeciderTests
{
    public class Decide
    {
        private static readonly FlatRatingStrategy Rating = new();

        private static Player MakePlayer(double hiddenRating) => new()
        {
            Id = Guid.NewGuid(),
            Name = $"P{hiddenRating}",
            HiddenRating = hiddenRating,
            RatingState = Rating.CreateInitialState(hiddenRating)
        };

        // ------------------------------------------------------------------
        // Deterministic outcomes (win probability is 0 or 1)
        // ------------------------------------------------------------------

        [Fact]
        public void TeamAWinsWhenTeamBHasZeroHiddenRating()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(1000) };
            var teamB = new List<Player> { MakePlayer(0) };
            var match = new Match(teamA, teamB);

            var (winners, _) = sut.Decide(match);

            Assert.Equal(teamA, winners);
        }

        [Fact]
        public void TeamBWinsWhenTeamAHasZeroHiddenRating()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(0) };
            var teamB = new List<Player> { MakePlayer(1000) };
            var match = new Match(teamA, teamB);

            var (winners, _) = sut.Decide(match);

            Assert.Equal(teamB, winners);
        }

        [Fact]
        public void LosersAreOpposingTeamWhenTeamAWins()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(1000) };
            var teamB = new List<Player> { MakePlayer(0) };
            var match = new Match(teamA, teamB);

            var (_, losers) = sut.Decide(match);

            Assert.Equal(teamB, losers);
        }

        [Fact]
        public void LosersAreOpposingTeamWhenTeamBWins()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(0) };
            var teamB = new List<Player> { MakePlayer(1000) };
            var match = new Match(teamA, teamB);

            var (_, losers) = sut.Decide(match);

            Assert.Equal(teamA, losers);
        }

        // ------------------------------------------------------------------
        // Edge cases
        // ------------------------------------------------------------------

        [Fact]
        public void DoesNotThrowWhenBothTeamsHaveZeroHiddenRating()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var match = new Match([MakePlayer(0)], [MakePlayer(0)]);

            var exception = Record.Exception(() => sut.Decide(match));

            Assert.Null(exception);
        }

        [Fact]
        public void WinnersAndLosersDoNotOverlapWhenBothTeamsHaveZeroHiddenRating()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(0) };
            var teamB = new List<Player> { MakePlayer(0) };
            var match = new Match(teamA, teamB);

            var (winners, losers) = sut.Decide(match);

            Assert.Empty(winners.Intersect(losers));
        }

        [Fact]
        public void UsesAverageHiddenRatingAcrossMultipleTeamMembers()
        {
            // TeamA avg = (1000 + 0) / 2 = 500, TeamB avg = 0 → TeamA always wins
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(1000), MakePlayer(0) };
            var teamB = new List<Player> { MakePlayer(0), MakePlayer(0) };
            var match = new Match(teamA, teamB);

            var (winners, _) = sut.Decide(match);

            Assert.Equal(teamA, winners);
        }

        // ------------------------------------------------------------------
        // Statistical outcomes
        // ------------------------------------------------------------------

        [Fact]
        public void EqualHiddenRatingsProduceApproximatelyFiftyPercentWinRate()
        {
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(1000) };
            var teamB = new List<Player> { MakePlayer(1000) };
            var match = new Match(teamA, teamB);

            var teamAWins = CountWins(sut, match, teamA, trials: 1000);

            Assert.InRange(teamAWins / 1000.0, 0.42, 0.58);
        }

        [Fact]
        public void StrongerTeamWinsProportionallyMoreOften()
        {
            // TeamA hidden=1500, TeamB hidden=500 → win probability = 1500/2000 = 75%
            var sut = new HiddenRatingOutcomeDecider(new Random(42));
            var teamA = new List<Player> { MakePlayer(1500) };
            var teamB = new List<Player> { MakePlayer(500) };
            var match = new Match(teamA, teamB);

            var teamAWins = CountWins(sut, match, teamA, trials: 2000);

            Assert.InRange(teamAWins / 2000.0, 0.68, 0.82);
        }

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        private static int CountWins(
            HiddenRatingOutcomeDecider sut,
            Match match,
            List<Player> targetTeam,
            int trials)
        {
            var wins = 0;
            for (var i = 0; i < trials; i++)
            {
                var (winners, _) = sut.Decide(match);
                if (winners == targetTeam) wins++;
            }
            return wins;
        }
    }
}
