namespace FastMiniApis.Test;

public class TestFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Response.Headers.Add("X-Test", "TestFilter");
        return next(context);
    }
}