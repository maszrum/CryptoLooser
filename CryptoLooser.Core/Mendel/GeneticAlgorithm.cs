namespace CryptoLooser.Core.Mendel;

public class GeneticAlgorithm<TState>
{
    private readonly SortedCollection<double, Individual<TState>> _population;
    private readonly IMutator _mutator;
    private readonly IFitnessProvider<TState> _fitnessProvider;
    private readonly GeneticAlgorithmParams _params;

    public GeneticAlgorithm(
        GeneticAlgorithmParams algorithmParams,
        IChromosomeFactory chromosomeFactory,
        IMutator mutator,
        IFitnessProvider<TState> fitnessProvider)
    {
        _mutator = mutator;
        _fitnessProvider = fitnessProvider;
        _params = algorithmParams;

        var individuals = Enumerable
            .Repeat(0,  algorithmParams.PopulationSize)
            .Select(_ => chromosomeFactory.GetNextChromosome())
            .Select(CalculateFitnessAndGetIndividual);

        _population = new SortedCollection<double, Individual<TState>>(
            items: individuals,
            valueToKeyFunc: i => i.Fitness,
            comparer: new DoubleReverseComparer());

        BestEver = _population.GetMaxValue();
    }

    public int Generation { get; private set; }

    public Individual<TState> BestEver { get; private set; }

    public event Func<Individual<TState>, Task>? BestEverChanged;

    public async Task NextGeneration()
    {
        Generation++;

        var firstMax = _population.PopMaxValue();
        var secondMax = _population.PopMaxValue();

        if (firstMax.Fitness > BestEver.Fitness)
        {
            BestEver = firstMax;

            if (BestEverChanged is not null)
            {
                await BestEverChanged(firstMax);
            }
        }

        var (newChromosome1, newChromosome2) = Crossover(firstMax.Chromosome, secondMax.Chromosome);

        if (_mutator.SpinTheWheel(newChromosome1))
        {
            _mutator.Mutate(newChromosome1);
        }

        if (_mutator.SpinTheWheel(newChromosome2))
        {
            _mutator.Mutate(newChromosome2);
        }

        var fitness1 = _fitnessProvider.GetFitness(newChromosome1, out var state1);
        var fitness2 = _fitnessProvider.GetFitness(newChromosome2, out var state2);

        var newIndividual1 = new Individual<TState>(newChromosome1, fitness1, state1);
        var newIndividual2 = new Individual<TState>(newChromosome2, fitness2, state2);

        _population.Add(newIndividual1);
        _population.Add(newIndividual2);

        if (Generation % _params.InsertBestIndividualGenerationInterval == 1)
        {
            _population.RemoveMinValue();
            _population.Add(BestEver);
        }
    }

    private static (double[], double[]) Crossover(ReadOnlySpan<double> i1, ReadOnlySpan<double> i2)
    {
        var crossoverPoint = Random.Shared.Next() % (i1.Length - 1) + 1;

        var i1Part1 = i1.Slice(0, crossoverPoint);
        var i1Part2 = i1.Slice(crossoverPoint);
        var i2Part1 = i2.Slice(0, crossoverPoint);
        var i2Part2 = i2.Slice(crossoverPoint);

        var i3 = new double[i1.Length];
        var i4 = new double[i1.Length];

        i1Part1.CopyTo(i3.AsSpan());
        i2Part2.CopyTo(i3.AsSpan(crossoverPoint));
        i2Part1.CopyTo(i4.AsSpan());
        i1Part2.CopyTo(i4.AsSpan(crossoverPoint));

        return (i3, i4);
    }

    private Individual<TState> CalculateFitnessAndGetIndividual(double[] chromosome)
    {
        var fitness = _fitnessProvider.GetFitness(chromosome, out var state);
        return new Individual<TState>(chromosome, fitness, state);
    }
}
