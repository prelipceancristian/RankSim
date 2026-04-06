using RankSim.Core.Models;

namespace RankSim.Core.Queue;

public sealed class QueueContext
{
    private QueueContext(Player player, int currentTick, int ticksInQueue, int ticksSinceLastMatch)
    {
        Player = player;
        CurrentTick = currentTick;
        TicksInQueue = ticksInQueue;
        TicksSinceLastMatch = ticksSinceLastMatch;
    }

    public Player Player { get; init; }
    public int CurrentTick { get; init; }
    public int TicksInQueue { get; init; } // 0 if not in queue
    public int TicksSinceLastMatch { get; init; }

    // Note: the contexts are slightly different based on the context
    // the ticks in queue property is relevant only for the leave context
    // the ticks since last match is only for the join context
    // could just be separate objects, maybe with base abstract class for shared properties
    public static QueueContext GetJoinQueueContext(Player player, int currentTick)
    {
        return new QueueContext(player, currentTick, 0, player.LastMatchTick - currentTick);
    }

    public static QueueContext GetLeaveQueueContext(Player player, int joinTick, int currentTick)
    {
        return new QueueContext(player, currentTick, joinTick - currentTick, 0);
    }

#pragma warning disable S125 // Sections of code should not be commented out
    // public int QueueDepth { get; init; } = queueDepth;
}
#pragma warning restore S125 // Sections of code should not be commented out
