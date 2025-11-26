namespace Loom.Server.Configuration;

public sealed class ServerSettings
{
    public string DataDirectory { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".loom");

    public string ApiKey { get; set; } = "CHANGE_ME";
}
