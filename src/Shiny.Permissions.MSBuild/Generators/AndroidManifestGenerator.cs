using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Generators;

public static class AndroidManifestGenerator
{
    const string PermissionPrefix = "android.permission.";
    const string FeaturePrefix = "android.hardware.";
    const int MinSegmentsForFullyQualified = 3;

    public static string ResolvePermissionName(string rawName)
    {
        var segments = rawName.Split('.');
        return segments.Length >= MinSegmentsForFullyQualified
            ? rawName
            : PermissionPrefix + rawName;
    }

    public static string ResolveFeatureName(string rawName)
    {
        var segments = rawName.Split('.');
        return segments.Length >= MinSegmentsForFullyQualified
            ? rawName
            : FeaturePrefix + rawName;
    }

    public static string GeneratePermissionElement(AndroidPermission permission)
    {
        var name = ResolvePermissionName(permission.Name);
        var sb = new StringBuilder();
        sb.Append($"<uses-permission android:name=\"{name}\"");

        if (!string.IsNullOrWhiteSpace(permission.MinSdkVersion))
            sb.Append($" android:minSdkVersion=\"{permission.MinSdkVersion}\"");

        if (!string.IsNullOrWhiteSpace(permission.MaxSdkVersion))
            sb.Append($" android:maxSdkVersion=\"{permission.MaxSdkVersion}\"");

        sb.Append(" />");
        return sb.ToString();
    }

    public static string GenerateFeatureElement(AndroidFeature feature)
    {
        var name = ResolveFeatureName(feature.Name);
        var sb = new StringBuilder();
        sb.Append($"<uses-feature android:name=\"{name}\"");

        if (!string.IsNullOrWhiteSpace(feature.Required))
            sb.Append($" android:required=\"{feature.Required}\"");

        sb.Append(" />");
        return sb.ToString();
    }

    public static string GenerateManifestXml(
        IEnumerable<AndroidPermission> permissions,
        IEnumerable<AndroidFeature> features)
    {
        var permissionList = permissions.ToList();
        var featureList = features.ToList();

        ValidateNoDuplicatePermissions(permissionList);
        ValidateNoDuplicateFeatures(featureList);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\">");

        foreach (var permission in permissionList)
        {
            sb.Append("    ");
            sb.AppendLine(GeneratePermissionElement(permission));
        }

        foreach (var feature in featureList)
        {
            sb.Append("    ");
            sb.AppendLine(GenerateFeatureElement(feature));
        }

        sb.Append("</manifest>");
        return sb.ToString();
    }

    static void ValidateNoDuplicatePermissions(List<AndroidPermission> permissions)
    {
        var seen = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in permissions)
        {
            var resolved = ResolvePermissionName(p.Name);
            if (seen.TryGetValue(resolved, out var existingRaw))
                throw new InvalidOperationException(
                    $"Duplicate Android permission '{resolved}' (specified as '{existingRaw}' and '{p.Name}')");
            seen[resolved] = p.Name;
        }
    }

    static void ValidateNoDuplicateFeatures(List<AndroidFeature> features)
    {
        var seen = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in features)
        {
            var resolved = ResolveFeatureName(f.Name);
            if (seen.TryGetValue(resolved, out var existingRaw))
                throw new InvalidOperationException(
                    $"Duplicate Android feature '{resolved}' (specified as '{existingRaw}' and '{f.Name}')");
            seen[resolved] = f.Name;
        }
    }
}
