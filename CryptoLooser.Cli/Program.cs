using System.CommandLine;
using CryptoLooser.Cli;
using CryptoLooser.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .Build();

var neuralNetworkSettings = configuration
    .GetRequiredSection(NeuralNetworkSettings.ConfigurationKey)
    .Get<NeuralNetworkSettings>()!;

var geneticAlgorithmSettings = configuration
    .GetRequiredSection(GeneticAlgorithmSettings.ConfigurationKey)
    .Get<GeneticAlgorithmSettings>()!;

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var rootCommand = new RootCommand("Bunch of commands to lose money on cryptocurrency market.");
rootCommand.AddCommand(new LearnCommand(logger, neuralNetworkSettings, geneticAlgorithmSettings));

await rootCommand.InvokeAsync(args);
