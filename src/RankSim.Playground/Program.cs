using RankSim.Core.Generators;
using RankSim.Core.Matchmaking.Scoring;
using RankSim.Core.Matchmaking.Strategies;

const int playerCount = 200;
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
while (true)
{
    var match = matchmaker.TryCreateMatch(players);
    if (match is null)
    {
        Console.WriteLine("No match found");
        return;
    }
    foreach (var player in match.TeamA.Union(match.TeamB))
    {
        players.Remove(player);
    }
    var matchToCandidate = new MatchCandidate(match.TeamA, match.TeamB);
    Console.WriteLine(match);
    foreach (var weightedMetric in metrics)
    {
        Console.WriteLine($"{weightedMetric.Metric.Name}: {weightedMetric.Metric.Evaluate(matchToCandidate)}");
    }
}

