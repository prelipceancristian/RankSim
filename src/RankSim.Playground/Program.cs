using RankSim.Core.Generators;

const int playerCount = 100;
const int seed = 0;
#pragma warning disable S125
// const int simulationTicks = 1000;
#pragma warning restore S125

var randomGenerator = new Random(seed);
#pragma warning disable S125
// var flatRatingStrategy = new FlatRatingStrategy();
#pragma warning restore S125
//
// var players = GeneratePlayers(playerCount, randomGenerator, flatRatingStrategy);
//
// foreach (var player in players)
// {
//     Console.WriteLine(player);
// }
//
// var queueStrategy = new InterestBasedQueueStrategy();
// var waitingPool = new PlayerWaitingPool(queueStrategy);
//
// for (var tick = 0; tick < simulationTicks; tick++)
// {
//     waitingPool.ProcessTick(players, tick);
//     Console.WriteLine($"Tick {tick} - players in queue {waitingPool.PlayersInQueue}");
// }
//
// return;
//
// static List<Player> GeneratePlayers(int count, Random generator, IRatingStrategy ratingStrategy)
// {
//     return Enumerable.Range(0, count)
//         .Select(index => GeneratePlayer(index, generator, ratingStrategy))
//         .ToList();
// }
//
// static Player GeneratePlayer(int index, Random generator, IRatingStrategy ratingStrategy)
// {
//     var initialRating = generator.Next(1, 5000);
//     var gameInterest = generator.NextDouble();
//     return new Player
//     {
//         Id = Guid.NewGuid(),
//         Name = $"Player {index}",
//         RatingState = ratingStrategy.CreateInitialState(initialRating),
//         GameInterest = gameInterest,
//     };
// }

var playerGenerator = new PlayerGenerator(randomGenerator);
var players = Enumerable.Range(0, playerCount)
    .Select(_ => playerGenerator.Generate())
    .ToList();

foreach (var player in players.OrderByDescending(p => p.HiddenRating))
{
    Console.WriteLine(player);
}

