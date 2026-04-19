using RankSim.Core.Matchmaking;

namespace RankSim.Core.Engine;

public sealed class ActiveGame(Match match, int ticksRemaining)
{
    public Match Match { get; } = match;
    public int TicksRemaining { get; set; } = ticksRemaining;
}
