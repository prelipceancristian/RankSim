using RankSim.Core.Models;

namespace RankSim.Core.Matchmaking;

public interface IMatchmaker
{
    string Name { get; }
    Match? TryCreateMatch(IReadOnlyList<Player> pool);
}
