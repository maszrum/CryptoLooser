namespace CryptoLooser.Core.Mendel;

public interface IMutator
{
    bool SpinTheWheel(double[] chromosome);

    void Mutate(double[] chromosome);
}
