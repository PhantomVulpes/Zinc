namespace Vulpes.Zinc.Domain.Configuration;
public static class ApplicationConfiguration
{
    private static readonly string debugEnvironmentName = "Debug";
    private static readonly string qualEnvironmentName = "Qual";
    private static readonly string releaseEnvironmentName = "Release";

    public static string ApplicationName => "Zinc";
    public static string Environment
    {
        get
        {
            // Determine the environment based on the build configuration.
#if DEBUG
            return debugEnvironmentName;
#elif QUAL
            return qualEnvironmentName;
#elif RELEASE
            return releaseEnvironmentName;
#endif
        }
    }

    public static bool IsRelease => Environment == releaseEnvironmentName;

    public static string DatabaseName
    {
        get
        {
            var defaultName = ApplicationName;
#if DEBUG
            return $"{defaultName}-{Environment}";
#elif QUAL
            return $"{defaultName}-{qualEnvironmentName}";
#elif RELEASE
            return defaultName;
#endif
        }
    }

    public static string DatabaseConnectionString => "mongodb://localhost:60001/";

    public static string Version => "Alpha 0.2";
}