namespace CryptoLooser.Core.NeuralNetwork;

public record NeuralNetworkInputLengths(
    int ClosePrice,
    int PriceDifference,
    int CandlestickHeight,
    int Volume,
    int TradesCount)
{
    public int Sum() =>
        ClosePrice + PriceDifference + CandlestickHeight + Volume + TradesCount;
}
