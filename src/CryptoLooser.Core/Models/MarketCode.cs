namespace CryptoLooser.Core.Models;

public struct MarketCode
{
    public MarketCode(string @from, string to)
    {
        if (string.IsNullOrEmpty(@from))
        {
            throw new ArgumentException(
                "Cannot be empty.", nameof(@from));
        }
        
        if (string.IsNullOrEmpty(@to))
        {
            throw new ArgumentException(
                "Cannot be empty.", nameof(to));
        }
        
        if (!@from.All(char.IsLetter))
        {
            throw new ArgumentException(
                "Must contain letters only.", nameof(@from));
        }

        if (!to.All(char.IsLetter))
        {
            throw new ArgumentException(
                "Must contain letters only.", nameof(@to));
        }

        From = @from.ToUpperInvariant();
        To = to.ToUpperInvariant();
    }

    public string From { get; }

    public string To { get; }

    public override string ToString() => $"{From}-{To}";
    
    public static MarketCode Parse(string code)
    {
        var indexOfDash = code.IndexOf('-');
        
        if (indexOfDash == -1)
        {
            throw new ArgumentException(
                "Must contain one dash '-'.", nameof(code));
        }
        
        var @from = code.Substring(0, indexOfDash);
        var to = code.Substring(indexOfDash + 1);
        
        return new MarketCode(@from, to);
    }

    public static implicit operator string(MarketCode marketCode) => marketCode.ToString();
}