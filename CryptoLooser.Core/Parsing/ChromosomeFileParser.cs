using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using CryptoLooser.Core.NeuralNetwork;

namespace CryptoLooser.Core.Parsing;

public class ChromosomeFileParser
{
    public async Task<ChromosomeParsingOutput> Parse(string fileName)
    {
        var fileLines = await File.ReadAllLinesAsync(fileName);
        var genes = new double[fileLines.Length];

        var inputLengths = new NeuralNetworkInputLengths(0, 0, 0, 0, 0);
        var output = new ChromosomeParsingOutput {InputLengths = inputLengths};

        var genesCount = 0;

        foreach (var line in fileLines.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            if (line.StartsWith('#'))
            {
                if (TryExtractVariable(
                        line: line,
                        variableName: "HiddenLayerNeuronsCount",
                        parseFunc: x => int.Parse(x, NumberStyles.None, CultureInfo.InvariantCulture),
                        value: out var hiddenLayerNeuronsCount))
                {
                    output = output with {HiddenLayerNeuronsCount = hiddenLayerNeuronsCount};
                }
                else
                {
                    var foundTuple = typeof(NeuralNetworkInputLengths)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(property =>
                        {
                            var extracted = TryExtractVariable(
                                line,
                                property.Name,
                                x => int.Parse(x, NumberStyles.None, CultureInfo.InvariantCulture),
                                out var propertyValue);

                            return (Property: property, Extracted: extracted, PropertyValue: propertyValue);
                        })
                        .SingleOrDefault(t => t.Extracted);

                    if (foundTuple.Extracted)
                    {
                        foundTuple.Property.SetValue(inputLengths, foundTuple.PropertyValue);
                    }
                }
            }
            else
            {
                genes[genesCount] = double.Parse(
                    line,
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture);

                genesCount++;
            }
        }

        output = output with {Genes = genes.AsMemory(0, genesCount)};

        return output;
    }

    private static bool TryExtractVariable<T>(
        string line,
        string variableName,
        Func<string, T> parseFunc,
        [NotNullWhen(true)] out T? value) where T : notnull
    {
        if (line.Contains(variableName))
        {
            var indexOfEquals = line.IndexOf('=');
            var valuePart = line.AsSpan(indexOfEquals + 1).Trim().ToString();

            value = parseFunc(valuePart);
            return true;
        }

        value = default;
        return false;
    }
}
