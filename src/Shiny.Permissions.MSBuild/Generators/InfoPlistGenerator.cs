using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Generators;

public static class InfoPlistGenerator
{
    public static string GenerateEntry(InfoPlistEntry entry)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<key>{entry.Key}</key>");

        switch (entry.Type.ToLowerInvariant())
        {
            case "string":
                sb.Append($"<string>{entry.Value}</string>");
                break;

            case "boolean":
            case "bool":
                var boolVal = entry.Value.ToLowerInvariant();
                sb.Append(boolVal == "true" ? "<true/>" : "<false/>");
                break;

            case "array":
            case "stringarray":
                sb.AppendLine("<array>");
                var stringItems = entry.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in stringItems)
                {
                    sb.AppendLine($"    <string>{item.Trim()}</string>");
                }
                sb.Append("</array>");
                break;

            case "integerarray":
                sb.AppendLine("<array>");
                var intItems = entry.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in intItems)
                {
                    sb.AppendLine($"    <integer>{item.Trim()}</integer>");
                }
                sb.Append("</array>");
                break;

            default:
                sb.Append($"<string>{entry.Value}</string>");
                break;
        }

        return sb.ToString();
    }

    public static string GeneratePlistXml(IEnumerable<InfoPlistEntry> entries)
    {
        var entryList = entries.ToList();

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in entryList)
        {
            if (!seen.Add(entry.Key))
                throw new InvalidOperationException(
                    $"Duplicate Info.plist key '{entry.Key}'");
        }

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList1.0.dtd\">");
        sb.AppendLine("<plist version=\"1.0\">");
        sb.AppendLine("    <dict>");

        foreach (var entry in entryList)
        {
            var lines = GenerateEntry(entry).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                sb.Append("        ");
                sb.AppendLine(line);
            }
        }

        sb.AppendLine("    </dict>");
        sb.Append("</plist>");
        return sb.ToString();
    }
}
