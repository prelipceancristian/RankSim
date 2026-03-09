namespace RankSim.Core.HiddenRating;

/// <summary>
/// Models a new player: rapid early improvement that plateaus as they accumulate experience.
/// Skill gain follows a logarithmic curve (diminishing returns).
/// Interest starts high but dips after the initial burst, then stabilises.
/// </summary>
public sealed class NewPlayerStrategy : IHiddenRatingStrategy
{
    /// <summary>Scales the logarithmic skill gain (higher = faster improvement).</summary>
    public double LearningRate { get; init; } = 3.0;

    /// <summary>Match count at which skill gain has halved relative to the very first match.</summary>
    public int HalfLifeMatches { get; init; } = 50;

    /// <summary>Interest level the player settles at after the initial burst fades.</summary>
    public double BaselineInterest { get; init; } = 0.6;

    /// <summary>Matches played before interest starts to normalise.</summary>
    public int InterestBurstDuration { get; init; } = 20;

    public string Name => "NewPlayer";

    public void Apply(PlayerTickContext context)
    {
        // Logarithmic improvement: large gains early, diminishing with experience.
        var gain = LearningRate / (1.0 + (double)context.MatchesPlayed / HalfLifeMatches);
        context.HiddenRating += gain;

        // Interest stays near 1 during the burst window, then decays to baseline.
        if (context.MatchesPlayed < InterestBurstDuration)
        {
            context.GameInterest = Math.Min(1.0, context.GameInterest);
        }
        else
        {
            var overshoot = context.GameInterest - BaselineInterest;
            if (overshoot > 0)
                context.GameInterest -= overshoot * 0.05; // gradual convergence to baseline
        }
    }
}
