using RankSim.Core.Models;
using RankSim.Core.Queue.Strategies;

namespace RankSim.Core.Queue;

public class PlayerWaitingPool(IQueueStrategy queueStrategy) : IPlayerWaitingPool
{
    private readonly Dictionary<Guid, QueueEntry> _store = [];

    public QueueEntry? FindEntry(Player player)
    {
        return _store.GetValueOrDefault(player.Id);
    }

    public void Join(Player player, int currentTick)
    {
        if (_store.ContainsKey(player.Id)) return;
        var queueEntry = new QueueEntry(player, currentTick);
        _store.Add(player.Id, queueEntry);
    }

    public void Leave(Player player)
    {
        _ = _store.Remove(player.Id);
    }

    public void ProcessTick(IReadOnlyList<Player> allPlayers, int currentTick)
    {
        foreach (var player in allPlayers)
        {
            var playerEntry = FindEntry(player);
            if (playerEntry is null)
            {
                // determine if player should join
                var context = QueueContext.GetJoinQueueContext(player, currentTick);
                if (queueStrategy.ShouldJoin(context))
                {
                    Join(player, currentTick);
                }
            }
            else
            {
                var context = QueueContext.GetLeaveQueueContext(player, playerEntry.JoinedAtTick, currentTick);
                if (queueStrategy.ShouldLeave(context))
                {
                    Leave(player);
                }
            }
        }
    }

    public int PlayersInQueue => _store.Count;
}
