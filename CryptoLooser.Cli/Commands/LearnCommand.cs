using System.Collections.Immutable;
using System.CommandLine;
using CryptoLooser.Core;
using CryptoLooser.Core.Mendel;
using CryptoLooser.Core.Parsing;
using Serilog;

namespace CryptoLooser.Cli.Commands;

internal class LearnCommand : Command
{
    private const string CommandName = "learn";
    private const string CommandDescription = "Start learning neural network with generic algorithm.";

    private readonly ILogger _logger;
    private readonly NeuralNetworkSettings _neuralNetworkSettings;
    private readonly GeneticAlgorithmSettings _geneticAlgorithmSettings;

    public LearnCommand(
        ILogger logger,
        NeuralNetworkSettings neuralNetworkSettings,
        GeneticAlgorithmSettings geneticAlgorithmSettings)
        : base(CommandName, CommandDescription)
    {
        _logger = logger;
        _neuralNetworkSettings = neuralNetworkSettings;
        _geneticAlgorithmSettings = geneticAlgorithmSettings;

        this.SetHandler(context => Handle(context.GetCancellationToken()));
    }

    private async Task Handle(CancellationToken cancellationToken)
    {
        var marketFilesLocation = Path.Combine("market-data", "ethusdt");

        _logger.Information(
            "Reading and parsing market data files from location: {MarketFilesLocation}",
            marketFilesLocation);

        var parser = BinanceMarketDataFileParser.Create("ETHUSDT", marketFilesLocation);

        MarketDataRow[] marketData;
        try
        {
            marketData = await parser
                .GetMarketData(cancellationToken)
                .ToArrayAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        _logger.Information(
            "Loaded {MarketDataRowsCount} market data rows.",
            marketData.Length);

        var marketSimulation = new MarketSimulation(
            marketData: marketData.ToImmutableArray(),
            neuralNetworkHiddenLayerNeuronsCount: _neuralNetworkSettings.HiddenLayerNeuronsCount,
            neuralNetworkSeriesSize: _neuralNetworkSettings.SeriesSize);

        var chromosomeFactory = new RandomChromosomeFactory(marketSimulation.RequiredChromosomeLength);

        var geneticAlgorithmParams = new GeneticAlgorithmParams
        {
            PopulationSize = _geneticAlgorithmSettings.PopulationSize,
            InsertBestIndividualGenerationInterval = _geneticAlgorithmSettings.InsertBestIndividualGenerationInterval
        };

        _logger.Information(
            "Creating initial population with number of {IndividualsCount} individuals.",
            geneticAlgorithmParams.PopulationSize);

        var geneticAlgorithm = new GeneticAlgorithm<IndividualState>(
            geneticAlgorithmParams,
            chromosomeFactory: chromosomeFactory,
            mutator: new RandomMutator(
                chromosomeFactory,
                _geneticAlgorithmSettings.MutationProbability,
                _geneticAlgorithmSettings.MaxMutationPercentage),
            fitnessProvider: marketSimulation);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.Information(
            "Best individual in initial population. Fitness: {Fitness}, profit: {Profit}, number of decisions: {DecisionsCount}.",
            geneticAlgorithm.BestEver.Fitness,
            geneticAlgorithm.BestEver.State.Profit,
            geneticAlgorithm.BestEver.State.Decisions.Length);

        var exporter = new ChromosomeExporter(Directory.GetCurrentDirectory(), _neuralNetworkSettings);
        exporter.Exported += chromosomeFileName =>
        {
            _logger.Information(
                "Chromosome exported to file: {ChromosomeFileName}.",
                chromosomeFileName);
        };

        await exporter.Export(geneticAlgorithm.BestEver.GetGenes());

        geneticAlgorithm.BestEverChanged += async individual =>
        {
            _logger.Information(
                "New best individual found. Fitness: {Fitness}, profit: {Profit}, number of decisions: {DecisionsCount}.",
                individual.Fitness,
                individual.State.Profit,
                individual.State.Decisions.Length);

            await exporter.Export(geneticAlgorithm.BestEver.GetGenes());
        };

        var performanceCounter = new PeriodicPerformanceCounter(
            TimeSpan.FromSeconds(10),
            generationsPerSecond =>
            {
                _logger.Information(
                    "Current generation: {GenerationNumber}. Processing {GenerationsPerSecond} generations/s.",
                    geneticAlgorithm.Generation,
                    generationsPerSecond);
            });

        var counterTask = performanceCounter.Start(cancellationToken);

        _logger.Information("Starting iterating through generations...");

        while (
            !cancellationToken.IsCancellationRequested &&
            geneticAlgorithm.Generation < _geneticAlgorithmSettings.MaxGenerations)
        {
            await geneticAlgorithm.NextGeneration();
            performanceCounter.Increment();
        }

        await counterTask;

    }
}
