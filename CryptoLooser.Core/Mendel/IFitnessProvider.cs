namespace CryptoLooser.Core.Mendel;

public interface IFitnessProvider<TState>
{
    double GetFitness(ReadOnlySpan<double> chromosome, out TState state);
}
