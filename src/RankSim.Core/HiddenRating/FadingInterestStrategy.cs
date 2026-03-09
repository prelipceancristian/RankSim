namespace RankSim.Core.HiddenRating;

/// <summary>
/// Models a player who is gradually losing interest in the game.
/// Interest decays each tick; once interest drops below a threshold,
/// skill starts to erode (rust / loss of practice).
/// </summary>
public sealed class FadingInterestStrategy : IHiddenRatingStrategy
{
    /// <summary>Fractional interest lost per tick (applied to current interest).</summary>
    public double InterestDecayRate { get; init; } = 0.008;

    /// <summary>Interest level below which skill begins to decay.</summary>
    public double SkillDecayThreshold { get; init; } = 0.4;

    /// <summary>Skill lost per tick when interest is below the threshold (scaled by how far below).</summary>
    public double SkillDecayRate { get; init; } = 1.0;

    public string Name => "FadingInterest";

    public void Apply(PlayerTickContext context)
    {
        context.GameInterest = Math.Max(0.0, context.GameInterest - InterestDecayRate);

        if (context.GameInterest < SkillDecayThreshold)
        {
            var decayFactor = (SkillDecayThreshold - context.GameInterest) / SkillDecayThreshold;
            context.HiddenRating -= SkillDecayRate * decayFactor;
        }
    }
}
