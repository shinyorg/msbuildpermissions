using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tasks;

public class GenerateAndroidManifestTask : Task
{
    public ITaskItem[] Permissions { get; set; } = [];

    public ITaskItem[] Features { get; set; } = [];

    [Required]
    public string OutputPath { get; set; } = "";

    public override bool Execute()
    {
        if (Permissions.Length == 0 && Features.Length == 0)
            return true;

        var permissionModels = new AndroidPermission[Permissions.Length];
        for (var i = 0; i < Permissions.Length; i++)
        {
            var item = Permissions[i];
            permissionModels[i] = new AndroidPermission
            {
                Name = item.ItemSpec,
                MinSdkVersion = item.GetMetadata("minSdkVersion"),
                MaxSdkVersion = item.GetMetadata("maxSdkVersion")
            };
        }

        var featureModels = new AndroidFeature[Features.Length];
        for (var i = 0; i < Features.Length; i++)
        {
            var item = Features[i];
            featureModels[i] = new AndroidFeature
            {
                Name = item.ItemSpec,
                Required = item.GetMetadata("required")
            };
        }

        try
        {
            var xml = AndroidManifestGenerator.GenerateManifestXml(permissionModels, featureModels);

            var dir = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(OutputPath, xml);
            Log.LogMessage(MessageImportance.Normal, $"Generated AndroidManifest.xml at {OutputPath}");
        }
        catch (InvalidOperationException ex)
        {
            Log.LogError(ex.Message);
            return false;
        }

        return true;
    }
}
