namespace CryptoLooser.Core.Mendel;

public class RandomMutator(
    IChromosomeFactory chromosomeFactory,
    double mutationProbability,
    double maxMutationPercentage) : IMutator
{
    private readonly IChromosomeFactory _chromosomeFactory = chromosomeFactory;
    private readonly double _mutationProbability = mutationProbability;
    private readonly double _maxMutationPercentage = maxMutationPercentage;

    public bool SpinTheWheel(double[] chromosome) =>
        Random.Shared.NextDouble() < _mutationProbability;

    public void Mutate(double[] chromosome)
    {
        var mutationsCount = (int) (Random.Shared.Next() % (_maxMutationPercentage * chromosome.Length)) + 1;

        for (var i = 0; i < mutationsCount; i++)
        {
            var index = Random.Shared.Next() % chromosome.Length;
            chromosome[index] = _chromosomeFactory.GetNextGene();
        }
    }
}
