using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tasks;

public class GenerateAndroidManifestPermissionsTask : Task
{
    [Required]
    public ITaskItem[] Permissions { get; set; } = [];

    [Required]
    public string OutputPath { get; set; } = "";

    public override bool Execute()
    {
        var models = new AndroidPermission[Permissions.Length];
        for (var i = 0; i < Permissions.Length; i++)
        {
            var item = Permissions[i];
            models[i] = new AndroidPermission
            {
                Name = item.ItemSpec,
                MinSdkVersion = item.GetMetadata("minSdkVersion"),
                MaxSdkVersion = item.GetMetadata("maxSdkVersion")
            };
        }

        var xml = AndroidManifestPermissionGenerator.GenerateManifestXml(models);

        var dir = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(OutputPath, xml);
        Log.LogMessage(MessageImportance.Normal, $"Generated Android manifest permissions at {OutputPath}");
        return true;
    }
}
