namespace CryptoLooser.Core.Mendel;

internal class DoubleReverseComparer : IComparer<double>
{
    public int Compare(double x, double y) => y.CompareTo(x);
}
