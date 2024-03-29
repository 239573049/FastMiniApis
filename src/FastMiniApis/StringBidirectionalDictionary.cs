using FastMiniApis.Core;

namespace FastMiniApis;


[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class StringBidirectionalDictionary : BidirectionalDictionary<string, string>
{

    public StringBidirectionalDictionary() : base() { }

    public StringBidirectionalDictionary(Dictionary<string, string> firstToSecondDictionary)
        : base(firstToSecondDictionary)
    { }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public override bool ExistsInFirst(string value)
    {
        return base.ExistsInFirst(value.ToLowerInvariant());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public override bool ExistsInSecond(string value)
    {
        return base.ExistsInSecond(value.ToLowerInvariant());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public override string? GetFirstValue(string value)
    {
        return base.GetFirstValue(value.ToLowerInvariant());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public override string? GetSecondValue(string value)
    {
        return base.GetSecondValue(value.ToLowerInvariant());
    }

}
