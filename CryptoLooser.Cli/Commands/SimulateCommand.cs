using System.Collections.Immutable;
using System.CommandLine;
using CryptoLooser.Core;
using CryptoLooser.Core.Mendel;
using CryptoLooser.Core.Parsing;
using CryptoLooser.Core.Units;
using Serilog;
using Serilog.Events;

namespace CryptoLooser.Cli.Commands;

internal class SimulateCommand : Command
{
    private const string CommandName = "simulate";
    private const string CommandDescription = "Create neural network with given parameters and simulate decisions.";

    private readonly ILogger _logger;

    public SimulateCommand(ILogger logger) : base(CommandName, CommandDescription)
    {
        _logger = logger;

        var chromosomeArgument = new Argument<string>(
            name: "chromosome-input-name",
            description: "File name containing neural network parameters.");

        AddArgument(chromosomeArgument);

        this.SetHandler(Handle, chromosomeArgument);
    }

    private async Task Handle(string chromosomeFileName)
    {
        var parseOutput = await new ChromosomeFileParser().Parse(chromosomeFileName);

        var marketData = await CommandHelpers.LoadMarketData(_logger, CancellationToken.None);

        var simulation = new MarketSimulation(
            marketData.ToImmutableArray(),
            parseOutput.HiddenLayerNeuronsCount,
            parseOutput.SeriesSize);

        var startingBalance = 1000.0d.AsUsdt();
        var simulationOutput = simulation.Simulate(parseOutput.Genes.Span, startingBalance);

        _logger.Information("Simulation with provided neural network parameters finished.");
        _logger.Information("Starting balance: {StartingBalance}", startingBalance);
        _logger.Information("Profit: {SimulationProfit}", simulationOutput.Profit);
        _logger.Information("Number of decisions: {DecisionsCount}", simulationOutput.Decisions.Length);

        LogDecisions(simulationOutput.Decisions);

        var justHoldSimulationOutput = simulation.SimulateJustHold(startingBalance);

        _logger.Information("Simulation with 'just hold' strategy finished.");
        _logger.Information("Starting balance: {StartingBalance}", startingBalance);
        _logger.Information("Profit: {SimulationProfit}", justHoldSimulationOutput.Profit);
        _logger.Information("Number of decisions: {DecisionsCount}", justHoldSimulationOutput.Decisions.Length);

        LogDecisions(justHoldSimulationOutput.Decisions);

        var bestPossibleSimulationOutput = simulation.SimulateBestPossible(startingBalance);

        _logger.Information("Simulation with best possible decisions finished.");
        _logger.Information("Starting balance: {StartingBalance}", startingBalance);
        _logger.Information("Profit: {SimulationProfit}", bestPossibleSimulationOutput.Profit);
        _logger.Information(
            "Number of decisions: {DecisionsCount}",
            bestPossibleSimulationOutput.Decisions.Length);

        LogDecisions(bestPossibleSimulationOutput.Decisions);
    }

    private void LogDecisions(IEnumerable<TradeDecision> decisions)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            foreach (var (number, decision) in decisions.Select((td, i) => (i + 1, td)))
            {
                _logger.Debug(
                    "# {DecisionNumber}: {DecisionKind} at {DecisionPrice}",
                    number,
                    decision.Kind,
                    decision.Price);
            }
        }
    }
}
