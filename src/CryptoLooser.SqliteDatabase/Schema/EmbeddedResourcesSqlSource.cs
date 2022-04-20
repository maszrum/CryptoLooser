using System.Reflection;

namespace CryptoLooser.SqliteDatabase.Schema;

internal class EmbeddedResourcesSqlSource
{
    private readonly IReadOnlyList<(string TableName, string ResourceName)> _availableResources;

    private EmbeddedResourcesSqlSource(
        IReadOnlyList<(string FileName, string ResourceName)> availableResources)
    {
        _availableResources = availableResources;
    }

    public IEnumerable<string> GetAvailableTableNames() => 
        _availableResources.Select(r => r.TableName);

    public async Task<string> ReadSqlForTable(string tableName)
    {
        var assembly = Assembly.GetExecutingAssembly();
            
        var resource = _availableResources.SingleOrDefault(t => t.TableName == tableName);
        if (resource == default)
        {
            throw new ArgumentException(
                $"could not find resource file of table {tableName}", nameof(tableName));
        }

        await using var stream = assembly.GetManifestResourceStream(resource.ResourceName);
        if (stream is null)
        {
            throw new IOException(
                $"something went wrong, cannot create stream for embedded resource {resource.ResourceName}");
        }
            
        using var reader = new StreamReader(stream);
            
        return await reader.ReadToEndAsync();
    }

    public static EmbeddedResourcesSqlSource Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        var availableResources = ReadResourcesOrdered(resourceNames);

        return new EmbeddedResourcesSqlSource(availableResources);
    }

    private static IReadOnlyList<(string FileName, string ResourceName)> ReadResourcesOrdered(
        IReadOnlyCollection<string> resourceNames)
    {
        var resources = new List<(int Order, string TableName, string ResourceName)>(resourceNames.Count);
        foreach (var t in resourceNames)
        {
            var tableName = ResourceNameToTableName(t, out var order);

            if (!string.IsNullOrEmpty(tableName))
            {
                resources.Add((order, tableName, t));
            }
        }

        return resources
            .OrderBy(t => t.Order)
            .Select(t => (t.TableName, t.ResourceName))
            .ToArray();
    }

    private static string? ResourceNameToTableName(string resourceName, out int order)
    {
        // from Namespace.AnotherNamespace.directory.12_tablename.sql
        // to 12_tablename

        var resourceNameParts = resourceName.Split('.');

        if (resourceNameParts.Length < 2 || resourceNameParts[^1] != "sql")
        {
            order = 0;
            return default;
        }

        var fileName = resourceNameParts[^2];

        // from 12_tablename
        // to tablename
        // out order = 12

        var indexOfUnderscore = fileName.IndexOf('_');
        
        if (indexOfUnderscore == -1)
        {
            order = 0;
            return default;
        }
        
        var orderText = fileName.Substring(0, indexOfUnderscore);
        fileName = fileName.Substring(indexOfUnderscore + 1);
        
        return int.TryParse(orderText, out order) 
            ? fileName 
            : default;
    }
}