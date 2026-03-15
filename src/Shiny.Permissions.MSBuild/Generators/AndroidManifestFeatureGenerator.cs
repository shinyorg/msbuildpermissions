using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Generators;

public static class AndroidManifestFeatureGenerator
{
    const string DefaultPrefix = "android.hardware.";
    const int MinSegmentsForFullyQualified = 3;

    public static string ResolveName(string rawName)
    {
        var segments = rawName.Split('.');
        if (segments.Length >= MinSegmentsForFullyQualified)
            return rawName;

        return DefaultPrefix + rawName;
    }

    public static string GenerateElement(AndroidFeature feature)
    {
        var name = ResolveName(feature.Name);
        var sb = new StringBuilder();
        sb.Append($"<uses-feature android:name=\"{name}\"");

        if (!string.IsNullOrWhiteSpace(feature.Required))
            sb.Append($" android:required=\"{feature.Required}\"");

        sb.Append(" />");
        return sb.ToString();
    }

    public static string GenerateManifestXml(IEnumerable<AndroidFeature> features)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\">");

        foreach (var feature in features)
        {
            sb.Append("    ");
            sb.AppendLine(GenerateElement(feature));
        }

        sb.Append("</manifest>");
        return sb.ToString();
    }
}
