using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tasks;

public class GenerateInfoPlistTask : Task
{
    [Required]
    public ITaskItem[] Entries { get; set; } = [];

    [Required]
    public string OutputPath { get; set; } = "";

    public override bool Execute()
    {
        var models = new InfoPlistEntry[Entries.Length];
        for (var i = 0; i < Entries.Length; i++)
        {
            var item = Entries[i];
            models[i] = new InfoPlistEntry
            {
                Key = item.ItemSpec,
                Type = item.GetMetadata("Type"),
                Value = item.GetMetadata("Value")
            };
        }

        var xml = InfoPlistGenerator.GeneratePlistXml(models);

        var dir = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(OutputPath, xml);
        Log.LogMessage(MessageImportance.Normal, $"Generated Info.plist at {OutputPath}");
        return true;
    }
}
