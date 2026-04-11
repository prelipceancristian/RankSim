namespace RankSim.Core.Generators;

public class NormalDistribution(double mean = 0, double stdDev = 1) : IDistribution<double>
{
    public double Sample(Random rng)
    {
        // https://stackoverflow.com/questions/218060/random-gaussian-variables
        var u1 = 1.0-rng.NextDouble(); //uniform(0,1] random doubles
        var u2 = 1.0-rng.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                               Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        var randNormal =
            mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }
}
