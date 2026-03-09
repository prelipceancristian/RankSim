namespace RankSim.Core.Rating;

public interface IRatingState
{
    /// <summary>
    /// The player's displayed rating, used by the matchmaker and shown to players.
    /// For some strategies (e.g. Elo) this is a stored value; for others (e.g. TrueSkill, Glicko-2)
    /// it is a computed projection of the underlying parameters (e.g. μ - 3σ).
    /// </summary>
    double PublicRating { get; }
}
