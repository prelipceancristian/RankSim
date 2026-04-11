namespace RankSim.Core.Generators;

public interface IDistribution<out T>
{
    T Sample(Random rng);
}
