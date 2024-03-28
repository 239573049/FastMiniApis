using System.ComponentModel;
using FastMiniApis.Core;
using FastMiniApis.Core.Attributes;

namespace FastMiniApis.Test;

[Filter(typeof(TestFilter))]
public class TestService : ServiceBase
{
    [Description("Hello World!")]
    public async Task<string> GetAsync()
    {
        return "Hello World!";
    }
}