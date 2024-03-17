namespace CryptoLooser.Core.Mendel;

public class RandomChromosomeFactory(int chromosomeLength) : IChromosomeFactory
{
    private readonly int _chromosomeLength = chromosomeLength;

    public double[] GetNextChromosome()
    {
        return Enumerable
            .Repeat(0, _chromosomeLength)
            .Select(_ => Random.Shared.NextDouble() * 2.0d - 1.0d)
            .ToArray();
    }

    public double GetNextGene() => Random.Shared.NextDouble() * 2.0d - 1.0d;
}
