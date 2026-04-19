using RankSim.Core.Engine;
using RankSim.Core.Generators;
using RankSim.Core.HiddenRating;
using RankSim.Core.Matchmaking.Scoring;
using RankSim.Core.Matchmaking.Strategies;
using RankSim.Core.Queue;
using RankSim.Core.Queue.Strategies;
using RankSim.Core.Rating.Strategies;

const int playerCount = 2000;
const int seed = 0;

var randomGenerator = new Random(seed);

var playerGenerator = new PlayerGenerator(randomGenerator);
var players = Enumerable.Range(0, playerCount)
    .Select(_ => playerGenerator.Generate())
    .ToList();

var metrics = new List<WeightedMetric>
{
    new (new IntraTeamSpreadMetric(), 1),
    new (new TeamMmrDeltaMetric(), 1)
};

var matchmaker = new BalancedMatchmaker(5, metrics, randomGenerator);
var pool = new PlayerWaitingPool(new InterestBasedQueueStrategy());
var engine = new MatchmakingEngine(players, pool, matchmaker, new FlatRatingStrategy(), new FlatStrategy(),
    new HiddenRatingOutcomeDecider(randomGenerator), randomGenerator);
engine.Run();

foreach (var player in players)
{
    Console.WriteLine($"{player.Name}\r\n\t{player.RatingState.PublicRating}\r\n\t{player.HiddenRating}");
}
