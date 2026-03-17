using System;
using System.Collections.Generic;
using System.Linq;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Generators;

public static class MauiPermissionsGenerator
{
    const string DefaultUsageDescription = "Say something useful here that your users will understand";

    public static IReadOnlyList<string> KnownPermissions { get; } = new[]
    {
        "BluetoothLE", "Location", "LocationBackground", "Geofencing",
        "Push", "Microphone", "Contacts", "Calendar",
        "Camera", "Photos", "Maps", "Biometric"
    };

    public static bool IsKnownPermission(string name) => PermissionSets.ContainsKey(name);

    static readonly Dictionary<string, (AndroidPermission[] Permissions, AndroidFeature[] Features, InfoPlistEntry[] PlistEntries)> PermissionSets =
        BuildPermissionSets();

    static Dictionary<string, (AndroidPermission[] Permissions, AndroidFeature[] Features, InfoPlistEntry[] PlistEntries)> BuildPermissionSets()
    {
        var sets = new Dictionary<string, (AndroidPermission[], AndroidFeature[], InfoPlistEntry[])>(StringComparer.OrdinalIgnoreCase);

        sets["BluetoothLE"] = (
            new[]
            {
                new AndroidPermission { Name = "BLUETOOTH" },
                new AndroidPermission { Name = "BLUETOOTH_ADMIN" },
                new AndroidPermission { Name = "BLUETOOTH_CONNECT" },
                new AndroidPermission { Name = "BLUETOOTH_SCAN" },
                new AndroidPermission { Name = "ACCESS_COARSE_LOCATION" },
                new AndroidPermission { Name = "ACCESS_FINE_LOCATION" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "UIBackgroundModes", Type = "array", Value = "bluetooth-central" },
                new InfoPlistEntry { Key = "NSBluetoothAlwaysUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Location"] = (
            new[]
            {
                new AndroidPermission { Name = "ACCESS_COARSE_LOCATION" },
                new AndroidPermission { Name = "ACCESS_FINE_LOCATION" },
            },
            new[]
            {
                new AndroidFeature { Name = "LOCATION.GPS", Required = "false" },
                new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
            },
            new[]
            {
                new InfoPlistEntry { Key = "UIBackgroundModes", Type = "array", Value = "location" },
                new InfoPlistEntry { Key = "NSLocationAlwaysUsageDescription", Type = "string", Value = DefaultUsageDescription },
                new InfoPlistEntry { Key = "NSLocationWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
                new InfoPlistEntry { Key = "NSLocationAlwaysAndWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["LocationBackground"] = (
            new[]
            {
                new AndroidPermission { Name = "FOREGROUND_SERVICE_LOCATION" },
                new AndroidPermission { Name = "FOREGROUND_SERVICE" },
                new AndroidPermission { Name = "ACCESS_COARSE_LOCATION" },
                new AndroidPermission { Name = "ACCESS_FINE_LOCATION" },
            },
            new[]
            {
                new AndroidFeature { Name = "LOCATION.GPS", Required = "false" },
                new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
            },
            new[]
            {
                new InfoPlistEntry { Key = "UIBackgroundModes", Type = "array", Value = "location" },
                new InfoPlistEntry { Key = "NSLocationAlwaysUsageDescription", Type = "string", Value = DefaultUsageDescription },
                new InfoPlistEntry { Key = "NSLocationWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
                new InfoPlistEntry { Key = "NSLocationAlwaysAndWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Geofencing"] = (
            new[]
            {
                new AndroidPermission { Name = "ACCESS_BACKGROUND_LOCATION" },
                new AndroidPermission { Name = "ACCESS_COARSE_LOCATION" },
                new AndroidPermission { Name = "ACCESS_FINE_LOCATION" },
            },
            new[]
            {
                new AndroidFeature { Name = "LOCATION.GPS", Required = "false" },
                new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
            },
            new[]
            {
                new InfoPlistEntry { Key = "UIBackgroundModes", Type = "array", Value = "location" },
                new InfoPlistEntry { Key = "NSLocationAlwaysUsageDescription", Type = "string", Value = DefaultUsageDescription },
                new InfoPlistEntry { Key = "NSLocationWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
                new InfoPlistEntry { Key = "NSLocationAlwaysAndWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Push"] = (
            new[]
            {
                new AndroidPermission { Name = "POST_NOTIFICATIONS" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "UIBackgroundModes", Type = "array", Value = "remote-notification" },
            }
        );

        sets["Microphone"] = (
            new[]
            {
                new AndroidPermission { Name = "RECORD_AUDIO" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSMicrophoneUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Contacts"] = (
            new[]
            {
                new AndroidPermission { Name = "READ_CONTACTS" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSContactsUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Calendar"] = (
            new[]
            {
                new AndroidPermission { Name = "READ_CALENDAR" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSCalendarsUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Camera"] = (
            new[]
            {
                new AndroidPermission { Name = "CAMERA" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSCameraUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Photos"] = (
            new[]
            {
                new AndroidPermission { Name = "READ_EXTERNAL_STORAGE" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSPhotoLibraryUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Maps"] = (
            new[]
            {
                new AndroidPermission { Name = "ACCESS_FINE_LOCATION" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSLocationWhenInUseUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        sets["Biometric"] = (
            new[]
            {
                new AndroidPermission { Name = "USE_BIOMETRIC" },
            },
            Array.Empty<AndroidFeature>(),
            new[]
            {
                new InfoPlistEntry { Key = "NSFaceIDUsageDescription", Type = "string", Value = DefaultUsageDescription },
            }
        );

        return sets;
    }

    public static (List<AndroidPermission> Permissions, List<AndroidFeature> Features, List<InfoPlistEntry> PlistEntries)
        Resolve(IEnumerable<string> permissionNames)
        => Resolve(permissionNames, Array.Empty<InfoPlistEntry>());

    public static (List<AndroidPermission> Permissions, List<AndroidFeature> Features, List<InfoPlistEntry> PlistEntries)
        Resolve(IEnumerable<string> permissionNames, IEnumerable<InfoPlistEntry> additionalPlistEntries)
    {
        var allPermissions = new List<AndroidPermission>();
        var allFeatures = new List<AndroidFeature>();
        var allPlistEntries = new List<InfoPlistEntry>();

        foreach (var name in permissionNames)
        {
            if (!PermissionSets.TryGetValue(name, out var definition))
                throw new InvalidOperationException(
                    $"Unknown MAUI permission '{name}'. Known permissions: {string.Join(", ", KnownPermissions)}");

            allPermissions.AddRange(definition.Permissions);
            allFeatures.AddRange(definition.Features);
            allPlistEntries.AddRange(definition.PlistEntries);
        }

        // Additional entries are appended last so they override auto-generated defaults
        allPlistEntries.AddRange(additionalPlistEntries);

        return (
            DeduplicatePermissions(allPermissions),
            DeduplicateFeatures(allFeatures),
            MergePlistEntries(allPlistEntries)
        );
    }

    public static (string AndroidManifestXml, string InfoPlistXml) Generate(IEnumerable<string> permissionNames)
        => Generate(permissionNames, Array.Empty<InfoPlistEntry>());

    public static (string AndroidManifestXml, string InfoPlistXml) Generate(
        IEnumerable<string> permissionNames,
        IEnumerable<InfoPlistEntry> additionalPlistEntries)
    {
        var (permissions, features, plistEntries) = Resolve(permissionNames, additionalPlistEntries);

        var androidXml = AndroidManifestGenerator.GenerateManifestXml(permissions, features);
        var plistXml = InfoPlistGenerator.GeneratePlistXml(plistEntries);

        return (androidXml, plistXml);
    }

    static List<AndroidPermission> DeduplicatePermissions(List<AndroidPermission> permissions)
    {
        var seen = new Dictionary<string, AndroidPermission>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in permissions)
        {
            var resolved = AndroidManifestGenerator.ResolvePermissionName(p.Name);
            if (!seen.ContainsKey(resolved))
                seen[resolved] = p;
        }
        return seen.Values.ToList();
    }

    static List<AndroidFeature> DeduplicateFeatures(List<AndroidFeature> features)
    {
        var seen = new Dictionary<string, AndroidFeature>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in features)
        {
            var resolved = AndroidManifestGenerator.ResolveFeatureName(f.Name);
            if (!seen.ContainsKey(resolved))
                seen[resolved] = f;
        }
        return seen.Values.ToList();
    }

    static List<InfoPlistEntry> MergePlistEntries(List<InfoPlistEntry> entries)
    {
        var merged = new Dictionary<string, InfoPlistEntry>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in entries)
        {
            if (merged.TryGetValue(entry.Key, out var existing))
            {
                // Merge array entries by combining values, preserving insertion order
                if (existing.Type.Equals("array", StringComparison.OrdinalIgnoreCase) &&
                    entry.Type.Equals("array", StringComparison.OrdinalIgnoreCase))
                {
                    var existingValues = existing.Value
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => v.Trim())
                        .ToList();

                    var existingSet = new HashSet<string>(existingValues, StringComparer.OrdinalIgnoreCase);

                    foreach (var val in entry.Value
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => v.Trim()))
                    {
                        if (existingSet.Add(val))
                            existingValues.Add(val);
                    }

                    existing.Value = string.Join(";", existingValues);
                }
                // Non-array duplicate keys: last-seen wins so custom entries override defaults
                else
                {
                    merged[entry.Key] = new InfoPlistEntry
                    {
                        Key = entry.Key,
                        Type = entry.Type,
                        Value = entry.Value
                    };
                }
            }
            else
            {
                merged[entry.Key] = new InfoPlistEntry
                {
                    Key = entry.Key,
                    Type = entry.Type,
                    Value = entry.Value
                };
            }
        }

        return merged.Values.ToList();
    }
}
