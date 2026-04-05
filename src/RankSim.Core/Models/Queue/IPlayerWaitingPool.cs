namespace RankSim.Core.Models.Queue;

public interface IPlayerWaitingPool
{
    void Join(Player player, int currentTick);
    void Leave(Player player);
    QueueEntry? FindEntry(Player player);
    void ProcessTick(IReadOnlyList<Player> allPlayers, int currentTick);
}
