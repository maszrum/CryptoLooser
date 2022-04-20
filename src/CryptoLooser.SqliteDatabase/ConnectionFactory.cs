using System.Data.SQLite;

namespace CryptoLooser.SqliteDatabase;

public class ConnectionFactory
{
    private readonly SqliteDatabaseConfiguration _configuration;

    public ConnectionFactory(SqliteDatabaseConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<SQLiteConnection> OpenConnection()
    {
        var connection = new SQLiteConnection(GetConnectionString());
        await connection.OpenAsync();

        return connection;
    }

    private string GetConnectionString() => 
        $"Data Source={_configuration.DatabaseFileName}";
}