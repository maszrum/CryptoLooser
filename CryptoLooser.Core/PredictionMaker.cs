namespace CryptoLooser.Core;

internal class PredictionMaker(double predictDownBelow, double predictUpAbove)
{
    private readonly double _predictDownBelow = predictDownBelow;
    private readonly double _predictUpAbove = predictUpAbove;

    public MarketPrediction Predict(double neuralNetworkOutput)
    {
        if (neuralNetworkOutput < _predictDownBelow)
        {
            return MarketPrediction.DumpIt;
        }

        return neuralNetworkOutput > _predictUpAbove
            ? MarketPrediction.ToTheMoon
            : MarketPrediction.NotSure;
    }

    public static PredictionMaker CreateRandom()
    {
        var rand1 = Random.Shared.NextDouble();
        var rand2 = Random.Shared.NextDouble();

        return new PredictionMaker(
            Math.Min(rand1, rand2),
            Math.Max(rand1, rand2));
    }
}
