using System.Runtime;

namespace DataTypeMapping.Model
{
    public class MailSettings
    {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string AppName { get; set; } = string.Empty;
            public string AppEmail { get; set; } = string.Empty;
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; }
    }
}
