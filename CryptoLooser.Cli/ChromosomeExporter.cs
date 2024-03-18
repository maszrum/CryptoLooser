using System.Globalization;
using System.Reflection;
using System.Text;
using CryptoLooser.Core.NeuralNetwork;

namespace CryptoLooser.Cli;

internal class ChromosomeExporter(
    string exportLocation,
    NeuralNetworkSettings neuralNetworkSettings)
{
    private readonly string _exportLocation = exportLocation;
    private readonly NeuralNetworkSettings _neuralNetworkSettings = neuralNetworkSettings;

    public event Action<string>? Exported;

    public async Task Export(IEnumerable<double> genes)
    {
        var genesFormatted = FormatChromosome(genes);
        var fileName = $"chromosome-{Guid.NewGuid().ToString("N").Substring(0, 8)}.txt";
        var filePath = Path.Combine(_exportLocation, fileName);

        var settingsFormatted = FormatSettings(_neuralNetworkSettings);

        await File.WriteAllTextAsync(filePath, string.Concat(settingsFormatted, genesFormatted));

        Exported?.Invoke(fileName);
    }

    private static string FormatSettings(NeuralNetworkSettings neuralNetworkSettings)
    {
        var sb = new StringBuilder()
            .AppendLine("# Neural network parameters:")
            .Append("# ")
            .Append(nameof(neuralNetworkSettings.HiddenLayerNeuronsCount))
            .Append(" = ")
            .AppendLine(neuralNetworkSettings.HiddenLayerNeuronsCount.ToString());

        var properties = typeof(NeuralNetworkInputLengths)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            sb
                .Append("# ")
                .Append(property.Name)
                .Append(" = ")
                .AppendLine(property.GetValue(neuralNetworkSettings.InputLengths)!.ToString());
        }

        return sb
            .AppendLine()
            .ToString();
    }

    private static string FormatChromosome(IEnumerable<double> genes)
    {
        var genesFormatted = genes
            .Select(gene => gene.ToString(CultureInfo.InvariantCulture));

        var chromosomeFormatted = string.Join(Environment.NewLine, genesFormatted);

        return chromosomeFormatted;
    }
}
