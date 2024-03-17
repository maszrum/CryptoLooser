namespace CryptoLooser.Core.Mendel;

public interface IChromosomeFactory
{
    double[] GetNextChromosome();

    double GetNextGene();
}
