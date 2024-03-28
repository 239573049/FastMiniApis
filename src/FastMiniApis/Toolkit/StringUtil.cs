namespace FastMiniApis.Core.Toolkit;

public static class StringUtil
{
    public static string StartWithLower(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return char.ToLower(str[0]) + str.Substring(1);
    }

    public static string StartWithUpper(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return char.ToUpper(str[0]) + str.Substring(1);
    }

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return char.ToLower(str[0]) + str.Substring(1);
    }

    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return char.ToUpper(str[0]) + str.Substring(1);
    }

    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
            .ToLower();
    }

    public static string ToKebabCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString()))
            .ToLower();
    }

    public static string ToConstantCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
            .ToUpper();
    }

    public static string TrimStart(this string str, string trimStr)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return str.StartsWith(trimStr) ? str[trimStr.Length..] : str;
    }

    public static string TrimStart(this string str, string trimStr, StringComparison comparisonType)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return str.StartsWith(trimStr, comparisonType) ? str[trimStr.Length..] : str;
    }

    public static string TrimEnd(this string str, string trimStr, StringComparison comparisonType)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return str.EndsWith(trimStr, comparisonType) ? str[..^trimStr.Length] : str;
    }

    public static string TrimStart(this string str, params string[] trimStrs)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        foreach (var trimStr in trimStrs)
        {
            if (str.StartsWith(trimStr))
            {
                return str[trimStr.Length..];
            }
        }

        return str;
    }

    public static string TrimEnd(this string str, params string[] trimStrs)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        foreach (var trimStr in trimStrs)
        {
            if (str.EndsWith(trimStr))
            {
                return str[..^trimStr.Length];
            }
        }

        return str;
    }

    public static string Trim(this string str, params string[] trimStrs)
    {
        return str.TrimStart(trimStrs).TrimEnd(trimStrs);
    }


    public static string TrimEnd(this string str, string trimStr)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return str.EndsWith(trimStr) ? str[..^trimStr.Length] : str;
    }

    public static string Trim(this string str, string trimStr)
    {
        return str.TrimStart(trimStr).TrimEnd(trimStr);
    }
}