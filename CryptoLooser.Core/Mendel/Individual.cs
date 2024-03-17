namespace CryptoLooser.Core.Mendel;

public class Individual<TState>(
    double[] chromosome,
    double fitness,
    TState state)
{
    private readonly double[] _chromosome = chromosome;

    public ReadOnlySpan<double> Chromosome => _chromosome;

    public double Fitness { get; } = fitness;

    public TState State { get; } = state;

    public void Mutate(IMutator mutator)
    {
        mutator.Mutate(_chromosome);
    }

    public IEnumerable<double> GetGenes() => _chromosome;
}
