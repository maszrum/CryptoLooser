using Microsoft.Extensions.Configuration;

namespace CryptoLooser.Configuration;

public class ConfigurationProvider
{
    private readonly string _fileName;

    public ConfigurationProvider(string fileName)
    {
        _fileName = fileName;
    }

    public TConfiguration GetConfiguration<TConfiguration>(string section)
    {
        var configurationRoot = BuildConfiguration(_fileName);
        
        return configurationRoot
            .GetRequiredSection(section)
            .Get<TConfiguration>();
    }
    
    private static IConfigurationRoot BuildConfiguration(string fileName)
    {
        return new ConfigurationBuilder()
            .AddJsonFile(fileName)
            .Build();
    }
}