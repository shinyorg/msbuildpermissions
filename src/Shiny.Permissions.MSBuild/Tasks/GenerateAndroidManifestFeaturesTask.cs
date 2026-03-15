using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tasks;

public class GenerateAndroidManifestFeaturesTask : Task
{
    [Required]
    public ITaskItem[] Features { get; set; } = [];

    [Required]
    public string OutputPath { get; set; } = "";

    public override bool Execute()
    {
        var models = new AndroidFeature[Features.Length];
        for (var i = 0; i < Features.Length; i++)
        {
            var item = Features[i];
            models[i] = new AndroidFeature
            {
                Name = item.ItemSpec,
                Required = item.GetMetadata("required")
            };
        }

        var xml = AndroidManifestFeatureGenerator.GenerateManifestXml(models);

        var dir = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(OutputPath, xml);
        Log.LogMessage(MessageImportance.Normal, $"Generated Android manifest features at {OutputPath}");
        return true;
    }
}
