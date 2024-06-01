namespace FastMiniApis;


/// <summary>
/// 这个类同时提供单数和复数的服务，它接受单词对
/// 按照第一个词是单数，第二个词是复数的规则。
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class BidirectionalDictionary<TFirst, TSecond>()
    where TFirst : notnull
    where TSecond : notnull
{
    public Dictionary<TFirst, TSecond> FirstToSecondDictionary { get; set; } = new();
    public Dictionary<TSecond, TFirst> SecondToFirstDictionary { get; set; } = new();

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

        SecondToFirstDictionary.TryAdd(secondValue, firstValue);
    }
}
