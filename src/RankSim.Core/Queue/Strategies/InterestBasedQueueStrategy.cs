namespace RankSim.Core.Queue.Strategies;

public class InterestBasedQueueStrategy : IQueueStrategy
{
    // NOTE: would be interesting at some point to see how general game interest might affect results,
    // maybe higher MinInterestToJoin causes fewer players to queue at times, leading to less accurate matches
    private const int MinWaitTolerance = 5;
    private const int MaxWaitTolerance = 50;
    private const int MinRejoinCooldown = 1;
    private const int MaxRejoinCooldown = 10;
    private const double MinInterestToJoin = 0.1;

    public string Name => "Strategy for acting based on the player's interest parameters";

    public bool ShouldJoin(QueueContext context)
    {
        // players with a low interest will never join
        if (context.Player.GameInterest < MinInterestToJoin)
        {
            return false;
        }
        var joinCooldown = MinRejoinCooldown + (MaxRejoinCooldown - MinRejoinCooldown) * context.Player.GameInterest;
        return joinCooldown > context.TicksSinceLastMatch;
    }

    public bool ShouldLeave(QueueContext context)
    {
        var waitTolerance = MinWaitTolerance + (MaxWaitTolerance - MinWaitTolerance) * context.Player.GameInterest;
        return waitTolerance > context.TicksInQueue;
    }
}
