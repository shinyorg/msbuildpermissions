using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Generators;

public static class AndroidManifestPermissionGenerator
{
    const string DefaultPrefix = "android.permission.";
    const int MinSegmentsForFullyQualified = 3;

    public static string ResolveName(string rawName)
    {
        var segments = rawName.Split('.');
        if (segments.Length >= MinSegmentsForFullyQualified)
            return rawName;

        return DefaultPrefix + rawName;
    }

    public static string GenerateElement(AndroidPermission permission)
    {
        var name = ResolveName(permission.Name);
        var sb = new StringBuilder();
        sb.Append($"<uses-permission android:name=\"{name}\"");

        if (!string.IsNullOrWhiteSpace(permission.MinSdkVersion))
            sb.Append($" android:minSdkVersion=\"{permission.MinSdkVersion}\"");

        if (!string.IsNullOrWhiteSpace(permission.MaxSdkVersion))
            sb.Append($" android:maxSdkVersion=\"{permission.MaxSdkVersion}\"");

        sb.Append(" />");
        return sb.ToString();
    }

    public static string GenerateManifestXml(IEnumerable<AndroidPermission> permissions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\">");

        foreach (var permission in permissions)
        {
            sb.Append("    ");
            sb.AppendLine(GenerateElement(permission));
        }

        sb.Append("</manifest>");
        return sb.ToString();
    }
}
