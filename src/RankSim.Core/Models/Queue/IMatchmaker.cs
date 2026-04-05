namespace RankSim.Core.Models.Queue;

public interface IMatchmaker
{
    void Join(Player player, int currentTick);
    void Leave(Player player);
    QueueEntry? FindEntry(Player player);
    void ProcessTick(IReadOnlyList<Player> allPlayers, int currentTick);
}
