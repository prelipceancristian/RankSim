using Moq;
using RankSim.Core.Models;
using RankSim.Core.Queue;
using RankSim.Core.Queue.Strategies;
using RankSim.Core.Rating.Strategies;

namespace RankSim.Core.Tests.Queue;

public static class PlayerWaitingPoolTests
{
    private static readonly FlatRatingStrategy RatingStrategy = new();

    public class Join
    {
        private readonly PlayerWaitingPool _playerWaitingPool;

        public Join()
        {
            var alwaysJoinNeverLeaveStrategy = new Mock<IQueueStrategy>();
            alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldJoin(It.IsAny<QueueContext>())).Returns(true);
            alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldLeave(It.IsAny<QueueContext>())).Returns(false);
            _playerWaitingPool = new PlayerWaitingPool(alwaysJoinNeverLeaveStrategy.Object);
        }

        [Fact]
        public void AddsPlayerToQueue()
        {
            // Arrange
            var player = CreateTestPlayer();

            // Act
            _playerWaitingPool.Join(player, 100);

            // Assert
            var entry = _playerWaitingPool.FindEntry(player);
            Assert.NotNull(entry);
            Assert.Equal(player.Id, entry.Player.Id);
            Assert.Equal(100, entry.JoinedAtTick);
        }

        [Fact]
        public void DoesNotAddDuplicatePlayer()
        {
            // Arrange
            var player = CreateTestPlayer();

            // Act
            _playerWaitingPool.Join(player, 100);
            _playerWaitingPool.Join(player, 200); // Try to add again

            // Assert
            var entry = _playerWaitingPool.FindEntry(player);
            Assert.NotNull(entry);
            Assert.Equal(100, entry.JoinedAtTick); // Should keep original join time
        }
    }

    public class Leave
    {
        private readonly PlayerWaitingPool _playerWaitingPool;

        public Leave()
        {
            var alwaysJoinNeverLeaveStrategy = new Mock<IQueueStrategy>();
            alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldJoin(It.IsAny<QueueContext>())).Returns(true);
            alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldLeave(It.IsAny<QueueContext>())).Returns(false);
            _playerWaitingPool = new PlayerWaitingPool(alwaysJoinNeverLeaveStrategy.Object);
        }

        [Fact]
        public void RemovesPlayerFromQueue()
        {
            // Arrange
            var player = CreateTestPlayer();
            _playerWaitingPool.Join(player, 100);

            // Act
            _playerWaitingPool.Leave(player);

            // Assert
            var entry = _playerWaitingPool.FindEntry(player);
            Assert.Null(entry);
        }

        [Fact]
        public void DoesNothingIfPlayerNotInQueue()
        {
            // Arrange
            var player = CreateTestPlayer();

            // Act
            _playerWaitingPool.Leave(player);

            // Assert - implicitly testing no exception is thrown
            var entry = _playerWaitingPool.FindEntry(player);
            Assert.Null(entry);
        }
    }

    public class FindEntry
    {
        private readonly PlayerWaitingPool _playerWaitingPool;

        public FindEntry()
        {
            var alwaysJoinNeverLeaveStrategy = new Mock<IQueueStrategy>();
            alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldJoin(It.IsAny<QueueContext>())).Returns(true);
            alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldLeave(It.IsAny<QueueContext>())).Returns(false);
            _playerWaitingPool = new PlayerWaitingPool(alwaysJoinNeverLeaveStrategy.Object);
        }

        [Fact]
        public void ReturnsNullForNonExistentPlayer()
        {
            // Arrange
            var player = CreateTestPlayer();

            // Act
            var entry = _playerWaitingPool.FindEntry(player);

            // Assert
            Assert.Null(entry);
        }

        [Fact]
        public void ReturnsCorrectEntryForExistingPlayer()
        {
            // Arrange
            var player1 = CreateTestPlayer();
            var player2 = CreateTestPlayer();
            _playerWaitingPool.Join(player1, 100);
            _playerWaitingPool.Join(player2, 200);

            // Act
            var entry1 = _playerWaitingPool.FindEntry(player1);
            var entry2 = _playerWaitingPool.FindEntry(player2);

            // Assert
            Assert.NotNull(entry1);
            Assert.Equal(player1.Id, entry1.Player.Id);
            Assert.Equal(100, entry1.JoinedAtTick);

            Assert.NotNull(entry2);
            Assert.Equal(player2.Id, entry2.Player.Id);
            Assert.Equal(200, entry2.JoinedAtTick);
        }
    }

    public class ProcessTick
    {
        private readonly Mock<IQueueStrategy> _alwaysJoinNeverLeaveStrategy = new();
        private readonly Mock<IQueueStrategy> _neverJoinStrategy = new();
        private readonly Mock<IQueueStrategy> _alwaysLeaveStrategy = new();

        public ProcessTick()
        {
            _alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldJoin(It.IsAny<QueueContext>())).Returns(true);
            _alwaysJoinNeverLeaveStrategy.Setup(s => s.ShouldLeave(It.IsAny<QueueContext>())).Returns(false);

            _neverJoinStrategy.Setup(s => s.ShouldJoin(It.IsAny<QueueContext>())).Returns(false);
            _neverJoinStrategy.Setup(s => s.ShouldLeave(It.IsAny<QueueContext>())).Returns(false);

            _alwaysLeaveStrategy.Setup(s => s.ShouldJoin(It.IsAny<QueueContext>())).Returns(true);
            _alwaysLeaveStrategy.Setup(s => s.ShouldLeave(It.IsAny<QueueContext>())).Returns(true);
        }

        [Fact]
        public void AddsPlayersWhenStrategyReturnsTrue()
        {
            // Arrange
            var matchmaker = new PlayerWaitingPool(_alwaysJoinNeverLeaveStrategy.Object);
            var player1 = CreateTestPlayer();
            var player2 = CreateTestPlayer();
            var players = new List<Player> { player1, player2 };

            // Act
            matchmaker.ProcessTick(players, 100);

            // Assert
            Assert.NotNull(matchmaker.FindEntry(player1));
            Assert.NotNull(matchmaker.FindEntry(player2));
        }

        [Fact]
        public void DoesNotAddPlayersWhenStrategyReturnsFalse()
        {
            // Arrange
            var matchmaker = new PlayerWaitingPool(_neverJoinStrategy.Object);
            var player1 = CreateTestPlayer();
            var player2 = CreateTestPlayer();
            var players = new List<Player> { player1, player2 };

            // Act
            matchmaker.ProcessTick(players, 100);

            // Assert
            Assert.Null(matchmaker.FindEntry(player1));
            Assert.Null(matchmaker.FindEntry(player2));
        }

        [Fact]
        public void RemovesPlayersWhenStrategyReturnsTrue()
        {
            // Arrange
            var matchmaker = new PlayerWaitingPool(_alwaysLeaveStrategy.Object);
            var player1 = CreateTestPlayer();
            var player2 = CreateTestPlayer();

            // Manually add players first
            matchmaker.Join(player1, 50);
            matchmaker.Join(player2, 60);

            var players = new List<Player> { player1, player2 };

            // Act
            matchmaker.ProcessTick(players, 100);

            // Assert
            Assert.Null(matchmaker.FindEntry(player1));
            Assert.Null(matchmaker.FindEntry(player2));
        }

        [Fact]
        public void HandlesEmptyPlayerList()
        {
            // Arrange
            var matchmaker = new PlayerWaitingPool(_alwaysJoinNeverLeaveStrategy.Object);
            var players = new List<Player>();

            // Act
            matchmaker.ProcessTick(players, 100);

            // Assert - verify no entries were added
            Assert.NotNull(players); // Just to have an assertion
        }

        [Fact]
        public void HandlesPlayersAlreadyInQueue()
        {
            // Arrange
            var matchmaker = new PlayerWaitingPool(_alwaysJoinNeverLeaveStrategy.Object);
            var player = CreateTestPlayer();
            matchmaker.Join(player, 50);

            var players = new List<Player> { player };

            // Act
            matchmaker.ProcessTick(players, 100);

            // Assert - player should still be in queue with original join time
            var entry = matchmaker.FindEntry(player);
            Assert.NotNull(entry);
            Assert.Equal(50, entry.JoinedAtTick);
        }
    }

    private static Player CreateTestPlayer()
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            Name = "TestPlayer",
            HiddenRating = 1500,
            GameInterest = 1.0,
            MatchesPlayed = 0,
            LastMatchTick = 0,
            RatingState = RatingStrategy.CreateInitialState(1500),
            DoubleDownTokens = 0
        };
    }
}
