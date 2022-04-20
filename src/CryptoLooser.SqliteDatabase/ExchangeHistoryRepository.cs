using System.Collections.Immutable;
using CryptoLooser.Core.Models;

namespace CryptoLooser.SqliteDatabase;

public class ExchangeHistoryRepository
{
    private readonly ConnectionFactory _connectionFactory;

    public ExchangeHistoryRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<ImmutableArray<CandlestickChartEntry>> GetCandlestickChart()
    {
        await using var connection = await _connectionFactory.OpenConnection();
        
        throw new NotImplementedException();
    }
}