using RankSim.Core.Rating;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Tests.Rating;

public class FlatRatingStrategyTests
{
    public class CreateInitialState
    {
        private readonly FlatRatingStrategy _sut = new();

        [Fact]
        public void SetsPublicRatingToInitialValue()
        {
            var state = _sut.CreateInitialState(1000);

            Assert.Equal(1000, state.PublicRating);
        }
    }

    public class UpdateRatings
    {
        private readonly FlatRatingStrategy _sut = new();

        [Fact]
        public void WinnerGainsDefaultDelta()
        {
            var winners = new MatchTeam([_sut.CreateInitialState(1000)]);
            var losers = new MatchTeam([_sut.CreateInitialState(1000)]);

            var (updatedWinners, _) = _sut.UpdateRatings(winners, losers);

            Assert.Equal(1025, updatedWinners.States[0].PublicRating);
        }

        [Fact]
        public void LoserLosesDefaultDelta()
        {
            var winners = new MatchTeam([_sut.CreateInitialState(1000)]);
            var losers = new MatchTeam([_sut.CreateInitialState(1000)]);

            var (_, updatedLosers) = _sut.UpdateRatings(winners, losers);

            Assert.Equal(975, updatedLosers.States[0].PublicRating);
        }

        [Fact]
        public void AllTeamMembersUpdated()
        {
            var winners = new MatchTeam([_sut.CreateInitialState(1000), _sut.CreateInitialState(1200)]);
            var losers = new MatchTeam([_sut.CreateInitialState(800), _sut.CreateInitialState(900)]);

            var (updatedWinners, updatedLosers) = _sut.UpdateRatings(winners, losers);

            Assert.Equal(1025, updatedWinners.States[0].PublicRating);
            Assert.Equal(1225, updatedWinners.States[1].PublicRating);
            Assert.Equal(775, updatedLosers.States[0].PublicRating);
            Assert.Equal(875, updatedLosers.States[1].PublicRating);
        }

        [Fact]
        public void CustomDeltaAppliedCorrectly()
        {
            var sut = new FlatRatingStrategy(delta: 50);
            var winners = new MatchTeam([sut.CreateInitialState(1000)]);
            var losers = new MatchTeam([sut.CreateInitialState(1000)]);

            var (updatedWinners, updatedLosers) = sut.UpdateRatings(winners, losers);

            Assert.Equal(1050, updatedWinners.States[0].PublicRating);
            Assert.Equal(950, updatedLosers.States[0].PublicRating);
        }

        [Fact]
        public void DoesNotMutateInputStates()
        {
            var winnerState = _sut.CreateInitialState(1000);
            var loserState = _sut.CreateInitialState(1000);
            var winners = new MatchTeam([winnerState]);
            var losers = new MatchTeam([loserState]);

            _sut.UpdateRatings(winners, losers);

            Assert.Equal(1000, winnerState.PublicRating);
            Assert.Equal(1000, loserState.PublicRating);
        }
    }
}
