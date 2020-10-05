public static Cake.Core.Configuration.ICakeConfiguration GetConfiguration(this ICakeContext context)
{
    var configProvider = new Cake.Core.Configuration.CakeConfigurationProvider(context.FileSystem, context.Environment);
    var arguments = (IDictionary<string, string>)context.Arguments.GetType().GetProperty("Arguments").GetValue(context.Arguments);
    return configProvider.CreateConfiguration(
        context.Environment.WorkingDirectory,
        arguments
        );
}
