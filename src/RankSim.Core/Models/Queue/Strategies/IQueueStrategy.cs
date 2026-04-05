namespace RankSim.Core.Models.Queue.Strategies;

public interface IQueueStrategy
{
    string Name { get; }
    bool ShouldJoin(QueueContext context);
    bool ShouldLeave(QueueContext context);
}
