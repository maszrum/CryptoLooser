namespace CryptoLooser.Cli;

internal class GeneticAlgorithmSettings
{
    public const string ConfigurationKey = "GeneticAlgorithm";

    public required int PopulationSize { get; init; }

    public required double MutationProbability { get; init; }

    public required double MaxMutationPercentage { get; init; }

    public required int InsertBestIndividualGenerationInterval { get; init; }

    public required int MaxGenerations { get; init; }
}
