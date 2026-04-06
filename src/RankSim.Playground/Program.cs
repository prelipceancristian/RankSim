using RankSim.Core.Models;
using RankSim.Core.Models.Queue;
using RankSim.Core.Models.Queue.Strategies;
using RankSim.Core.Rating;
using RankSim.Core.Rating.Strategies;

const int playerCount = 100;
const int seed = 0;
const int simulationTicks = 1000;

var randomGenerator = new Random(seed);
var flatRatingStrategy = new FlatRatingStrategy();

var players = GeneratePlayers(playerCount, randomGenerator, flatRatingStrategy);

foreach (var player in players)
{
    Console.WriteLine(player);
}

var queueStrategy = new InterestBasedQueueStrategy();
var waitingPool = new PlayerWaitingPool(queueStrategy);

for (var tick = 0; tick < simulationTicks; tick++)
{
    waitingPool.ProcessTick(players, tick);
    Console.WriteLine($"Tick {tick} - players in queue {waitingPool.PlayersInQueue}");
}

return;

static List<Player> GeneratePlayers(int count, Random generator, IRatingStrategy ratingStrategy)
{
    return Enumerable.Range(0, count)
        .Select(index => GeneratePlayer(index, generator, ratingStrategy))
        .ToList();
}

static Player GeneratePlayer(int index, Random generator, IRatingStrategy ratingStrategy)
{
    var initialRating = generator.Next(1, 5000);
    var gameInterest = generator.NextDouble();
    return new Player
    {
        Id = Guid.NewGuid(),
        Name = $"Player {index}",
        RatingState = ratingStrategy.CreateInitialState(initialRating),
        GameInterest = gameInterest,
    };
}

