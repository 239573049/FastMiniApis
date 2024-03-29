namespace System;

[AttributeUsage(AttributeTargets.Method)]
public class RoutePatternAttribute : Attribute
{
    public string? Pattern { get; set; }

    /// <summary>
    /// The request method, the default is null (the request method is automatically identified according to the method name prefix)
    /// </summary>
    public string? HttpMethod { get; set; }

    public bool StartWithBaseUri { get; set; }

    public RoutePatternAttribute(string? pattern = null, bool startWithBaseUri = false)
    {
        Pattern = pattern;
        StartWithBaseUri = startWithBaseUri;
    }
}