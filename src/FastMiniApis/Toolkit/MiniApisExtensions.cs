namespace FastMiniApis.Core.Toolkit;

public static class MiniApisExtensions
{
    public static TBuilder Description<TBuilder>(this TBuilder builder, string? description)
        where TBuilder : IEndpointConventionBuilder
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return builder;
        }

        return builder
            .WithDescription(description);
    }
}