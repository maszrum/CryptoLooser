using System.Collections.Immutable;
using CryptoLooser.Cli;
using CryptoLooser.Core;
using CryptoLooser.Core.Mendel;
using CryptoLooser.Core.Parsing;
using Microsoft.Extensions.Configuration;
using Serilog;

var programCts = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    programCts.Cancel();
    eventArgs.Cancel = true;
};

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .Build();

var neuralNetworkSettings = configuration
    .GetRequiredSection(NeuralNetworkSettings.ConfigurationKey)
    .Get<NeuralNetworkSettings>()!;

var geneticAlgorithmSettings = configuration
    .GetRequiredSection(GeneticAlgorithmSettings.ConfigurationKey)
    .Get<GeneticAlgorithmSettings>()!;

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var marketFilesLocation = Path.Combine("market-data", "ethusdt");

logger.Information(
    "Reading and parsing market data files from location: {MarketFilesLocation}",
    marketFilesLocation);

var parser = BinanceMarketDataFileParser.Create("ETHUSDT", marketFilesLocation);

MarketDataRow[] marketData;
try
{
    marketData = await parser
        .GetMarketData(programCts.Token)
        .ToArrayAsync(programCts.Token);
}
catch (OperationCanceledException)
{
    return;
}


logger.Information(
    "Loaded {MarketDataRowsCount} market data rows.",
    marketData.Length);

var marketSimulation = new MarketSimulation(
    marketData: marketData.ToImmutableArray(),
    neuralNetworkHiddenLayerNeuronsCount: neuralNetworkSettings.HiddenLayerNeuronsCount,
    neuralNetworkSeriesSize: neuralNetworkSettings.SeriesSize);

var chromosomeFactory = new RandomChromosomeFactory(marketSimulation.RequiredChromosomeLength);

var geneticAlgorithmParams = new GeneticAlgorithmParams
{
    PopulationSize = geneticAlgorithmSettings.PopulationSize,
    InsertBestIndividualGenerationInterval = geneticAlgorithmSettings.InsertBestIndividualGenerationInterval
};

logger.Information(
    "Creating initial population with number of {IndividualsCount} individuals.",
    geneticAlgorithmParams.PopulationSize);

var geneticAlgorithm = new GeneticAlgorithm<IndividualState>(
    geneticAlgorithmParams,
    chromosomeFactory: chromosomeFactory,
    mutator: new RandomMutator(
        chromosomeFactory,
        geneticAlgorithmSettings.MutationProbability,
        geneticAlgorithmSettings.MaxMutationPercentage),
    fitnessProvider: marketSimulation);

if (programCts.Token.IsCancellationRequested)
{
    return;
}

logger.Information(
    "Best individual in initial population. Fitness: {Fitness}, profit: {Profit}, number of decisions: {DecisionsCount}.",
    geneticAlgorithm.BestEver.Fitness,
    geneticAlgorithm.BestEver.State.Profit,
    geneticAlgorithm.BestEver.State.Decisions.Length);

var exporter = new ChromosomeExporter(Directory.GetCurrentDirectory(), neuralNetworkSettings);
exporter.Exported += chromosomeFileName =>
{
    logger.Information(
        "Chromosome exported to file: {ChromosomeFileName}.",
        chromosomeFileName);
};

await exporter.Export(geneticAlgorithm.BestEver.GetGenes());

geneticAlgorithm.BestEverChanged += async individual =>
{
    logger.Information(
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
        logger.Information(
            "Current generation: {GenerationNumber}. Processing {GenerationsPerSecond} generations/s.",
            geneticAlgorithm.Generation,
            generationsPerSecond);
    });

var counterTask = performanceCounter.Start(programCts.Token);

logger.Information("Starting iterating through generations...");

while (
    !programCts.Token.IsCancellationRequested &&
    geneticAlgorithm.Generation < geneticAlgorithmSettings.MaxGenerations)
{
    await geneticAlgorithm.NextGeneration();
    performanceCounter.Increment();
}

await counterTask;
