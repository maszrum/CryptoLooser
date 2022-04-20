using Dapper;

namespace CryptoLooser.SqliteDatabase.Schema;

public class DatabaseInitializer
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly SqliteDatabaseConfiguration _configuration;

    public DatabaseInitializer(
        ConnectionFactory connectionFactory,
        SqliteDatabaseConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
    }

    public async Task Initialize(bool force = false)
    {
        if (force)
        {
            RemoveExistingDatabaseIfExists();
        }

        await using var connection = await _connectionFactory.OpenConnection();
        
        var sqlSource = EmbeddedResourcesSqlSource.Initialize();
        
        var expectedTables = sqlSource.GetAvailableTableNames();

        foreach (var expectedTable in expectedTables)
        {
            var query = await sqlSource.ReadSqlForTable(expectedTable);
            
            _ = await connection.ExecuteAsync(query);
        }
    }

    private void RemoveExistingDatabaseIfExists()
    {
        if (File.Exists(_configuration.DatabaseFileName))
        {
            File.Delete(_configuration.DatabaseFileName);
        }
    }
}