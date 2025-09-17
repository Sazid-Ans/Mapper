using Serilog.Core;

namespace Upstream.Shared.InfraStructure.Logging
{
    public static class ApplicationConstants
    {
        public static readonly string ApplicationName;
        public static readonly string EnvironmentName;
        public static readonly string OperatingSystem;
        public static readonly string ProcessId;
        public static readonly LoggingLevelSwitch ConsoleLogLevel;

        static ApplicationConstants()
        {
            ApplicationName = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name!;
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            OperatingSystem = Environment.OSVersion.Platform.ToString();
            ProcessId = Environment.ProcessId.ToString();
            ConsoleLogLevel = new LoggingLevelSwitch();
        }
    }

}
