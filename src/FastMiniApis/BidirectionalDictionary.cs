namespace FastMiniApis.Core;


/// <summary>
/// This class provide service for both the singularization and pluralization, it takes the word pairs
/// in the ctor following the rules that the first one is singular and the second one is plural.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class BidirectionalDictionary<TFirst, TSecond>
    where TFirst : notnull
    where TSecond : notnull
{
    public Dictionary<TFirst, TSecond> FirstToSecondDictionary { get; set; }
    public Dictionary<TSecond, TFirst> SecondToFirstDictionary { get; set; }

    public BidirectionalDictionary()
    {
        FirstToSecondDictionary = new Dictionary<TFirst, TSecond>();
        SecondToFirstDictionary = new Dictionary<TSecond, TFirst>();
    }

    public BidirectionalDictionary(Dictionary<TFirst, TSecond> firstToSecondDictionary) : this()
    {
        foreach (var key in firstToSecondDictionary.Keys)
        {
            AddValue(key, firstToSecondDictionary[key]);
        }
    }

    public virtual bool ExistsInFirst(TFirst value)
    {
        return FirstToSecondDictionary.ContainsKey(value);
    }

    public virtual bool ExistsInSecond(TSecond value)
    {
        return SecondToFirstDictionary.ContainsKey(value);
    }

    public virtual TSecond? GetSecondValue(TFirst value)
    {
        if (ExistsInFirst(value))
            return FirstToSecondDictionary[value];

        return default;
    }

    public virtual TFirst? GetFirstValue(TSecond value)
    {
        if (ExistsInSecond(value))
            return SecondToFirstDictionary[value];

        return default;
    }

    public virtual void AddValue(TFirst firstValue, TSecond secondValue)
    {
        FirstToSecondDictionary.Add(firstValue, secondValue);

        if (!SecondToFirstDictionary.ContainsKey(secondValue))
        {
            SecondToFirstDictionary.Add(secondValue, firstValue);
        }
    }
}
