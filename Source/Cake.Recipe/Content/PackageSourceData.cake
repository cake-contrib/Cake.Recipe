public enum FeedType
{
    Chocolatey,
    NuGet
}

public class PackageSourceData
{
    public string Name { get; set; }
    public string PushUrl { get; set; }
    public FeedType Type { get; set; }
    public bool IsRelease { get; set; }
    public PackageSourceCredentials Credentials { get; private set; }

    public PackageSourceData(ICakeContext context, string name, string pushUrl)
        : this(context, name, pushUrl, FeedType.NuGet)
    {    
    }

    public PackageSourceData(ICakeContext context, string name, string pushUrl, FeedType feedType)
        : this(context, name, pushUrl, feedType, true)
    {
    }

    public PackageSourceData(ICakeContext context, string name, string pushUrl, FeedType feedType, bool isRelease)
    {
        Name = name;
        PushUrl = pushUrl;
        Type = feedType;
        IsRelease = isRelease;

        Credentials = new PackageSourceCredentials(
            context.EnvironmentVariable(Name.ToUpperInvariant() + "_API_KEY"),
            context.EnvironmentVariable(Name.ToUpperInvariant() + "_USER"),
            context.EnvironmentVariable(Name.ToUpperInvariant() + "_PASSWORD")
        );
    }
}