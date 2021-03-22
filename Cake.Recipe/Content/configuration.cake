public static Cake.Core.Configuration.ICakeConfiguration GetConfiguration(this ICakeContext context)
{
    var configProvider = new Cake.Core.Configuration.CakeConfigurationProvider(context.FileSystem, context.Environment);
    
    // This is very much a hack until this issue is implemented:
    // https://github.com/cake-build/cake/issues/2690
    var arguments = (Dictionary<string, List<string>>)context.Arguments.GetType().GetField("_arguments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(context.Arguments);

    return configProvider.CreateConfiguration(
        context.Environment.WorkingDirectory,
        arguments.ToDictionary(x => x.Key, y=>y.Value.LastOrDefault())
        );
}
