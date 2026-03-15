namespace Shiny.Permissions.MSBuild.Models;

public class AndroidPermission
{
    public string Name { get; set; } = "";
    public string? MinSdkVersion { get; set; }
    public string? MaxSdkVersion { get; set; }
}
