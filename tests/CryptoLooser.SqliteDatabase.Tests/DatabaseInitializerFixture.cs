using NUnit.Framework;
using CryptoLooser.SqliteDatabase.Schema;
using Dapper;

namespace CryptoLooser.SqliteDatabase.Tests;

[TestFixture]
public class DatabaseInitializerFixture
{
    [Test]
    public async Task check_if_initialization_creates_tables()
    {
        var databaseConfiguration = new SqliteDatabaseConfiguration("TestDb.db");
        var connectionFactory = new ConnectionFactory(databaseConfiguration);
        var initializer = new DatabaseInitializer(connectionFactory, databaseConfiguration);
        
        await initializer.Initialize(force: true);
        
        var expectedTables = EmbeddedResourcesSqlSource
            .Initialize()
            .GetAvailableTableNames();
        
        await using var connection = await connectionFactory.OpenConnection();
        
        var sql = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
        var actualTables = await connection.QueryAsync<string>(sql);
        
        CollectionAssert.AreEquivalent(expectedTables, actualTables);
    }
}