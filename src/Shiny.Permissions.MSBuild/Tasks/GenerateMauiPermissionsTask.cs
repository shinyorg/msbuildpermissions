using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tasks;

public class GenerateMauiPermissionsTask : Task
{
    [Required]
    public ITaskItem[] Permissions { get; set; } = [];

    [Required]
    public string AndroidManifestOutputPath { get; set; } = "";

    [Required]
    public string InfoPlistOutputPath { get; set; } = "";

    public override bool Execute()
    {
        if (Permissions.Length == 0)
            return true;

        var permissionSetNames = new List<string>();
        var customPlistEntries = new List<InfoPlistEntry>();

        foreach (var item in Permissions)
        {
            var type = item.GetMetadata("Type");
            var value = item.GetMetadata("Value");
            var hasTypeValue = !string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(value);

            if (MauiPermissionsGenerator.IsKnownPermission(item.ItemSpec))
            {
                permissionSetNames.Add(item.ItemSpec);
            }
            else if (hasTypeValue)
            {
                customPlistEntries.Add(new InfoPlistEntry
                {
                    Key = item.ItemSpec,
                    Type = type,
                    Value = value
                });
            }
            else
            {
                Log.LogError(
                    $"Unknown MauiPermission '{item.ItemSpec}'. " +
                    $"Known permissions: {string.Join(", ", MauiPermissionsGenerator.KnownPermissions)}. " +
                    "Custom plist entries require Type and Value metadata.");
                return false;
            }
        }

        try
        {
            var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(permissionSetNames, customPlistEntries);

            WriteFile(AndroidManifestOutputPath, androidXml);
            Log.LogMessage(MessageImportance.Normal, $"Generated AndroidManifest.xml at {AndroidManifestOutputPath}");

            WriteFile(InfoPlistOutputPath, plistXml);
            Log.LogMessage(MessageImportance.Normal, $"Generated Info.plist at {InfoPlistOutputPath}");
        }
        catch (InvalidOperationException ex)
        {
            Log.LogError(ex.Message);
            return false;
        }

        return true;
    }

    static void WriteFile(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, content);
    }
}
