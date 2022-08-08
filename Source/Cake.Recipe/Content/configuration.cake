public static Cake.Core.Configuration.ICakeConfiguration GetConfiguration(this ICakeContext context)
{
    var configProvider = new Cake.Core.Configuration.CakeConfigurationProvider(context.FileSystem, context.Environment);
    
    return configProvider.CreateConfiguration(
        context.Environment.WorkingDirectory,
        context.Arguments().ToDictionary(x => x.Key, y=>y.Value.LastOrDefault())
        );
}
