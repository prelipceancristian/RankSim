using RankSim.Core.Generators;
using RankSim.Core.HiddenRating;
using RankSim.Core.Matchmaking;
using RankSim.Core.Models;
using RankSim.Core.Queue;
using RankSim.Core.Rating;

namespace RankSim.Core.Engine;

public sealed class MatchmakingEngine(
    IReadOnlyList<Player> players,
    IPlayerWaitingPool pool,
    IMatchmaker matchmaker,
    IRatingStrategy ratingStrategy,
    IHiddenRatingStrategy hiddenRatingStrategy,
    IOutcomeDecider outcomeDecider,
    Random random)
{
    public const int TickCount = 10000;

    private readonly NormalDistribution _gameDurationDistribution = new(mean: 40, stdDev: 10);

    public void Run()
    {
        var activeGames = new List<ActiveGame>();

        for (var tick = 0; tick < TickCount; tick++)
        {
            pool.ProcessTick(players, tick);
            TryStartMatch(activeGames);
            AdvanceAndResolveGames(activeGames, tick);
        }

        DrainRemainingGames(activeGames);
    }

    private void TryStartMatch(List<ActiveGame> activeGames)
    {
        var match = matchmaker.TryCreateMatch(pool.Players.ToList());
        if (match is null) return;

        var duration = Math.Max(10, (int)_gameDurationDistribution.Sample(random));
        activeGames.Add(new ActiveGame(match, duration));

        foreach (var player in match.TeamA.Concat(match.TeamB))
            pool.Leave(player);
    }

    private void AdvanceAndResolveGames(List<ActiveGame> activeGames, int tick)
    {
        foreach (var game in activeGames)
            game.TicksRemaining--;

        var finished = activeGames.Where(g => g.TicksRemaining <= 0).ToList();
        foreach (var game in finished)
        {
            ResolveGame(game.Match, tick);
            activeGames.Remove(game);
        }
    }

    private void DrainRemainingGames(List<ActiveGame> activeGames)
    {
        while (activeGames.Count > 0)
            AdvanceAndResolveGames(activeGames, TickCount);
    }

    private void ResolveGame(Match match, int currentTick)
    {
        var (winners, losers) = outcomeDecider.Decide(match);
        ApplyRatingUpdates(winners, losers);
        ApplyPostMatchEffects(match, currentTick);
    }

    private void ApplyRatingUpdates(IReadOnlyList<Player> winners, IReadOnlyList<Player> losers)
    {
        var winnerTeam = new MatchTeam(winners.Select(p => p.RatingState).ToList());
        var loserTeam = new MatchTeam(losers.Select(p => p.RatingState).ToList());
        var (newWinnerTeam, newLoserTeam) = ratingStrategy.UpdateRatings(winnerTeam, loserTeam);

        for (var i = 0; i < winners.Count; i++)
            winners[i].RatingState = newWinnerTeam.States[i];
        for (var i = 0; i < losers.Count; i++)
            losers[i].RatingState = newLoserTeam.States[i];
    }

    private void ApplyPostMatchEffects(Match match, int currentTick)
    {
        foreach (var player in match.TeamA.Concat(match.TeamB))
        {
            var context = new PlayerTickContext
            {
                HiddenRating = player.HiddenRating,
                GameInterest = player.GameInterest,
                TicksSinceLastMatch = 0,
                MatchesPlayed = player.MatchesPlayed,
            };
            hiddenRatingStrategy.Apply(context);

            player.HiddenRating = context.HiddenRating;
            player.GameInterest = context.GameInterest;
            player.MatchesPlayed++;
            player.LastMatchTick = currentTick;
        }
    }
}
