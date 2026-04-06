namespace RankSim.Core.Queue.Strategies;

public class AlwaysJoinQueueStrategy : IQueueStrategy
{
    public string Name => "Strategy for always joining the matchmaking queue";

    public bool ShouldJoin(QueueContext context)
    {
        return true;
    }

    public bool ShouldLeave(QueueContext context)
    {
        return false;
    }
}
