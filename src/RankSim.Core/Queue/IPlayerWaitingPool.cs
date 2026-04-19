using RankSim.Core.Models;

namespace RankSim.Core.Queue;

public interface IPlayerWaitingPool
{
    IEnumerable<Player> Players { get; }
    void Join(Player player, int currentTick);
    void Leave(Player player);
    QueueEntry? FindEntry(Player player);
    void ProcessTick(IReadOnlyList<Player> allPlayers, int currentTick);
}
