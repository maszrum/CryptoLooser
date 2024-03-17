namespace CryptoLooser.Core.Mendel;

public class GeneticAlgorithmParams
{
    public required int PopulationSize { get; init; }

    public required int InsertBestIndividualGenerationInterval { get; init; }
}
