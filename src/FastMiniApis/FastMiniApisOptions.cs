namespace FastMiniApis.Core;

public class FastMiniApisOptions : ServiceRouteOptions
{
    public FastMiniApisOptions()
    {
        Prefix = "/api/" + Versions;

        GetPrefixes = new List<string> { "Get", "Select", "Find" };
        PostPrefixes = new List<string> { "Post", "Add", "Upsert", "Create", "Insert" };
        PutPrefixes = new List<string> { "Put", "Update", "Modify" };
        DeletePrefixes = new List<string> { "Delete", "Remove" };
    }

    public string Versions { get; set; } = "v1";

    public string Prefix { get; set; }


    public Func<string, (HttpMethod, string)>? RouteNameHandle { get; set; }


    internal PluralizationService Pluralization { get; set; } = new EnglishPluralizationService();
}