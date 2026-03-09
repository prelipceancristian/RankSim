using RankSim.Core.HiddenRating;

namespace RankSim.Core.Tests.HiddenRating;

public class HiddenRatingStrategyTests
{
    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static PlayerTickContext MakeContext(
        double hiddenRating = 1000,
        double gameInterest = 1.0,
        int ticksSinceLastMatch = 0,
        int matchesPlayed = 0) => new()
    {
        HiddenRating = hiddenRating,
        GameInterest = gameInterest,
        TicksSinceLastMatch = ticksSinceLastMatch,
        MatchesPlayed = matchesPlayed,
    };

    // ------------------------------------------------------------------
    // FlatStrategy
    // ------------------------------------------------------------------

    public class Flat
    {
        private readonly FlatStrategy _sut = new();

        [Fact]
        public void HiddenRatingUnchanged()
        {
            var ctx = MakeContext(hiddenRating: 500);
            _sut.Apply(ctx);
            Assert.Equal(500, ctx.HiddenRating);
        }

        [Fact]
        public void GameInterestUnchanged()
        {
            var ctx = MakeContext(gameInterest: 0.7);
            _sut.Apply(ctx);
            Assert.Equal(0.7, ctx.GameInterest);
        }
    }

    // ------------------------------------------------------------------
    // GradualImprovementStrategy
    // ------------------------------------------------------------------

    public class GradualImprovement
    {
        private readonly GradualImprovementStrategy _sut = new();

        [Fact]
        public void HiddenRatingIncreasesWhenActive()
        {
            var ctx = MakeContext(ticksSinceLastMatch: 0);
            _sut.Apply(ctx);
            Assert.True(ctx.HiddenRating > 1000);
        }

        [Fact]
        public void HiddenRatingGainScalesWithInterest()
        {
            var lowInterest = MakeContext(gameInterest: 0.2, ticksSinceLastMatch: 0);
            var highInterest = MakeContext(gameInterest: 0.9, ticksSinceLastMatch: 0);

            _sut.Apply(lowInterest);
            _sut.Apply(highInterest);

            Assert.True(highInterest.HiddenRating > lowInterest.HiddenRating);
        }

        [Fact]
        public void GameInterestIncreasesWhileActive()
        {
            var ctx = MakeContext(gameInterest: 0.8, ticksSinceLastMatch: 0);
            _sut.Apply(ctx);
            Assert.True(ctx.GameInterest > 0.8);
        }

        [Fact]
        public void GameInterestDecreasesWhenIdle()
        {
            var ctx = MakeContext(gameInterest: 0.8, ticksSinceLastMatch: 5);
            _sut.Apply(ctx);
            Assert.True(ctx.GameInterest < 0.8);
        }

        [Fact]
        public void GameInterestNeverExceedsOne()
        {
            var ctx = MakeContext(gameInterest: 1.0, ticksSinceLastMatch: 0);
            _sut.Apply(ctx);
            Assert.True(ctx.GameInterest <= 1.0);
        }

        [Fact]
        public void GameInterestNeverBelowZero()
        {
            var ctx = MakeContext(gameInterest: 0.001, ticksSinceLastMatch: 99);
            for (var i = 0; i < 500; i++)
                _sut.Apply(ctx);
            Assert.True(ctx.GameInterest >= 0.0);
        }
    }

    // ------------------------------------------------------------------
    // FadingInterestStrategy
    // ------------------------------------------------------------------

    public class FadingInterest
    {
        private readonly FadingInterestStrategy _sut = new();

        [Fact]
        public void GameInterestDecreasesEachTick()
        {
            var ctx = MakeContext(gameInterest: 0.8);
            _sut.Apply(ctx);
            Assert.True(ctx.GameInterest < 0.8);
        }

        [Fact]
        public void GameInterestNeverBelowZero()
        {
            var ctx = MakeContext(gameInterest: 0.0);
            _sut.Apply(ctx);
            Assert.True(ctx.GameInterest >= 0.0);
        }

        [Fact]
        public void HiddenRatingUnchangedWhenInterestAboveThreshold()
        {
            // Default SkillDecayThreshold = 0.4; start well above it.
            var ctx = MakeContext(hiddenRating: 1000, gameInterest: 0.9);
            _sut.Apply(ctx);
            Assert.Equal(1000, ctx.HiddenRating);
        }

        [Fact]
        public void HiddenRatingDecreasesWhenInterestBelowThreshold()
        {
            // Start just below the threshold so skill should erode.
            var ctx = MakeContext(hiddenRating: 1000, gameInterest: 0.1);
            _sut.Apply(ctx);
            Assert.True(ctx.HiddenRating < 1000);
        }

        [Fact]
        public void HiddenRatingDecayProportionalToInterestDeficit()
        {
            // Deeper below the threshold → larger skill loss per tick.
            var slightlyBelow = MakeContext(hiddenRating: 1000, gameInterest: 0.35);
            var farBelow = MakeContext(hiddenRating: 1000, gameInterest: 0.05);

            _sut.Apply(slightlyBelow);
            _sut.Apply(farBelow);

            Assert.True(farBelow.HiddenRating < slightlyBelow.HiddenRating);
        }
    }

    // ------------------------------------------------------------------
    // NewPlayerStrategy
    // ------------------------------------------------------------------

    public class NewPlayer
    {
        private readonly NewPlayerStrategy _sut = new();

        [Fact]
        public void HiddenRatingIncreasesEachTick()
        {
            var ctx = MakeContext(matchesPlayed: 0);
            _sut.Apply(ctx);
            Assert.True(ctx.HiddenRating > 1000);
        }

        [Fact]
        public void EarlyGainLargerThanLateGain()
        {
            var early = MakeContext(hiddenRating: 0, matchesPlayed: 0);
            var late = MakeContext(hiddenRating: 0, matchesPlayed: 500);

            _sut.Apply(early);
            _sut.Apply(late);

            Assert.True(early.HiddenRating > late.HiddenRating);
        }

        [Fact]
        public void GameInterestStaysAtOrBelowOneDuringBurst()
        {
            // Default InterestBurstDuration = 20
            var ctx = MakeContext(gameInterest: 1.0, matchesPlayed: 5);
            _sut.Apply(ctx);
            Assert.True(ctx.GameInterest <= 1.0);
        }

        [Fact]
        public void GameInterestConvergesToBaselineAfterBurst()
        {
            // Default BaselineInterest = 0.6, InterestBurstDuration = 20
            var ctx = MakeContext(gameInterest: 1.0, matchesPlayed: 100);

            // Run many ticks — interest should move toward 0.6.
            for (var i = 0; i < 200; i++)
                _sut.Apply(ctx);

            Assert.True(ctx.GameInterest < 1.0);
            Assert.True(ctx.GameInterest >= _sut.BaselineInterest - 0.01);
        }
    }
}
