namespace RankSim.Core.HiddenRating;

/// <summary>
/// Models an engaged player who steadily improves.
/// Skill grows each tick in proportion to their current interest level.
/// Interest decays slightly over time (sustained effort is hard to maintain),
/// but resets upward when the player has been active recently.
/// </summary>
public sealed class GradualImprovementStrategy : IHiddenRatingStrategy
{
    /// <summary>Base skill gain per tick at full interest.</summary>
    public double SkillGainPerTick { get; init; } = 0.5;

    /// <summary>How quickly interest fades when the player is inactive (per tick).</summary>
    public double InactivityDecayRate { get; init; } = 0.005;

    /// <summary>How much interest recovers per tick while the player is active (TicksSinceLastMatch == 0).</summary>
    public double ActivityRecoveryRate { get; init; } = 0.01;

    public string Name => "GradualImprovement";

    public void Apply(PlayerTickContext context)
    {
        context.HiddenRating += SkillGainPerTick * context.GameInterest;

        context.GameInterest = context.TicksSinceLastMatch == 0
            ? Math.Min(1.0, context.GameInterest + ActivityRecoveryRate)
            : Math.Max(0.0, context.GameInterest - InactivityDecayRate);
    }
}
