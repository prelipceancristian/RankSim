namespace RankSim.Core.Models.Queue;

public sealed class QueueEntry(Player player, int joinedAtTick)
{
    public Player Player { get; } = player;
    public int JoinedAtTick { get; } = joinedAtTick;
}
