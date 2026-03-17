---
name: shiny-permissions-msbuild
description: Generate Android manifest permissions/features and iOS Info.plist entries for .NET MAUI apps using Shiny.Permissions.MSBuild
auto_invoke: true
triggers:
  - maui permission
  - maui permissions
  - android permission
  - android manifest
  - ios plist
  - info plist
  - uses-permission
  - uses-feature
  - MauiPermission
  - AndroidManifestPermission
  - AndroidManifestFeature
  - InfoPlistPermission
  - NSCameraUsageDescription
  - NSLocationWhenInUseUsageDescription
  - NSBluetoothAlwaysUsageDescription
  - UIBackgroundModes
  - Shiny.Permissions.MSBuild
  - BluetoothLE permission
  - location permission
  - camera permission
  - push notification permission
  - biometric permission
---

# Shiny.Permissions.MSBuild Skill

You are an expert in Shiny.Permissions.MSBuild, an MSBuild task library that automatically generates Android manifest permissions/features and iOS Info.plist entries during the .NET MAUI build process.

## When to Use This Skill

Invoke this skill when the user wants to:
- Add platform permissions to a .NET MAUI project
- Configure Android manifest permissions or hardware features
- Configure iOS Info.plist usage description strings or background modes
- Use high-level `MauiPermission` items instead of manual platform XML
- Understand what Android/iOS entries a particular permission generates
- Debug permission-related build issues

## Library Overview

**Package:** `Shiny.Permissions.MSBuild`
**How it works:** Declare MSBuild items in `.csproj` → the build task generates platform XML into `$(IntermediateOutputPath)` before the Build target runs.

There are four item types:
1. `MauiPermission` — high-level permission sets that generate both Android and iOS entries
2. `AndroidManifestPermission` — individual Android `<uses-permission>` entries
3. `AndroidManifestFeature` — individual Android `<uses-feature>` entries
4. `InfoPlistPermission` — individual iOS Info.plist key/value entries

## MauiPermission — High-Level Permission Sets

This is the primary API. One item generates all required platform entries.

### Usage

```xml
<ItemGroup>
    <MauiPermission Include="Camera" />
    <MauiPermission Include="Location" />
    <MauiPermission Include="BluetoothLE" />
</ItemGroup>
```

### Known Permissions and Their Mappings

#### BluetoothLE
- **Android permissions:** `BLUETOOTH`, `BLUETOOTH_ADMIN`, `BLUETOOTH_CONNECT`, `BLUETOOTH_SCAN`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION`
- **Android features:** *(none)*
- **iOS:** `UIBackgroundModes` → `bluetooth-central`, `NSBluetoothAlwaysUsageDescription`

#### Location
- **Android permissions:** `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION`
- **Android features:** `LOCATION.GPS` (required=false), `LOCATION.NETWORK` (required=false)
- **iOS:** `UIBackgroundModes` → `location`, `NSLocationAlwaysUsageDescription`, `NSLocationWhenInUseUsageDescription`, `NSLocationAlwaysAndWhenInUseUsageDescription`

#### LocationBackground
- **Android permissions:** `FOREGROUND_SERVICE_LOCATION`, `FOREGROUND_SERVICE`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION`
- **Android features:** `LOCATION.GPS` (required=false), `LOCATION.NETWORK` (required=false)
- **iOS:** `UIBackgroundModes` → `location`, `NSLocationAlwaysUsageDescription`, `NSLocationWhenInUseUsageDescription`, `NSLocationAlwaysAndWhenInUseUsageDescription`

#### Geofencing
- **Android permissions:** `ACCESS_BACKGROUND_LOCATION`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION`
- **Android features:** `LOCATION.GPS` (required=false), `LOCATION.NETWORK` (required=false)
- **iOS:** `UIBackgroundModes` → `location`, `NSLocationAlwaysUsageDescription`, `NSLocationWhenInUseUsageDescription`, `NSLocationAlwaysAndWhenInUseUsageDescription`

#### Push
- **Android permissions:** `POST_NOTIFICATIONS`
- **iOS:** `UIBackgroundModes` → `remote-notification`

#### Microphone
- **Android permissions:** `RECORD_AUDIO`
- **iOS:** `NSMicrophoneUsageDescription`

#### Contacts
- **Android permissions:** `READ_CONTACTS`
- **iOS:** `NSContactsUsageDescription`

#### Calendar
- **Android permissions:** `READ_CALENDAR`
- **iOS:** `NSCalendarsUsageDescription`

#### Camera
- **Android permissions:** `CAMERA`
- **iOS:** `NSCameraUsageDescription`

#### Photos
- **Android permissions:** `READ_EXTERNAL_STORAGE`
- **iOS:** `NSPhotoLibraryUsageDescription`

#### Maps
- **Android permissions:** `ACCESS_FINE_LOCATION`
- **iOS:** `NSLocationWhenInUseUsageDescription`

#### Biometric
- **Android permissions:** `USE_BIOMETRIC`
- **iOS:** `NSFaceIDUsageDescription`

### Deduplication and Merging Rules

When multiple `MauiPermission` items are declared:
- **Android permissions** are deduplicated by resolved name (case-insensitive)
- **Android features** are deduplicated by resolved name (case-insensitive)
- **iOS array entries** (e.g. `UIBackgroundModes`) are merged — values from all permission sets are combined into a single array, preserving insertion order and removing duplicates
- **iOS non-array entries** use last-seen-wins — this allows custom entries to override auto-generated defaults

### Default Usage Description Strings

All iOS usage description strings default to: `"Say something useful here that your users will understand"`. Users should override these with real descriptions for App Store submission.

## AndroidManifestPermission — Fine-Grained Android Permissions

```xml
<ItemGroup>
    <AndroidManifestPermission Include="CAMERA" />
    <AndroidManifestPermission Include="ACCESS_FINE_LOCATION" />
    <AndroidManifestPermission Include="READ_EXTERNAL_STORAGE" MaxSdkVersion="32" />
    <AndroidManifestPermission Include="WRITE_EXTERNAL_STORAGE" MinSdkVersion="19" MaxSdkVersion="28" />
</ItemGroup>
```

**Metadata:** `MinSdkVersion` (optional), `MaxSdkVersion` (optional)

**Name resolution:** Short names are prefixed with `android.permission.`. Fully qualified names (3+ dot segments) are used as-is.

## AndroidManifestFeature — Fine-Grained Android Features

```xml
<ItemGroup>
    <AndroidManifestFeature Include="CAMERA" />
    <AndroidManifestFeature Include="LOCATION.GPS" Required="true" />
    <AndroidManifestFeature Include="BLUETOOTH" Required="false" />
</ItemGroup>
```

**Metadata:** `Required` (optional, `true` or `false`)

**Name resolution:** Short names are prefixed with `android.hardware.`. Fully qualified names (3+ dot segments) are used as-is.

## InfoPlistPermission — Fine-Grained iOS Info.plist Entries

```xml
<ItemGroup>
    <InfoPlistPermission Include="NSCameraUsageDescription" Type="string" Value="We need camera access" />
    <InfoPlistPermission Include="UIRequiresPersistentWiFi" Type="boolean" Value="true" />
    <InfoPlistPermission Include="LSApplicationQueriesSchemes" Type="stringarray" Value="fb;instagram;twitter" />
    <InfoPlistPermission Include="UIDeviceFamily" Type="integerarray" Value="1;2" />
</ItemGroup>
```

**Metadata:** `Type` (optional, default `string` — supports `string`, `boolean`/`bool`, `stringarray`, `integerarray`), `Value` (optional)

For arrays, separate items with `;`. Use `stringarray` for `<string>` elements and `integerarray` for `<integer>` elements. The legacy type name `array` is accepted as an alias for `stringarray`.

## Generated Output Files

All files are written to `$(IntermediateOutputPath)`:

| Item Group | Output File |
|---|---|
| `MauiPermission` | `AndroidManifest.xml` + `Info.plist` |
| `AndroidManifestPermission` / `AndroidManifestFeature` | `AndroidManifest.xml` |
| `InfoPlistPermission` | `Info.plist` |

## Architecture

```
src/Shiny.Permissions.MSBuild/
├── build/
│   ├── Shiny.Permissions.MSBuild.props    # Defines item groups
│   └── Shiny.Permissions.MSBuild.targets  # Registers tasks and targets
├── Generators/
│   ├── AndroidManifestGenerator.cs        # Android XML generation
│   ├── InfoPlistGenerator.cs              # iOS plist XML generation
│   └── MauiPermissionsGenerator.cs        # Permission set resolution + orchestration
├── Models/
│   ├── AndroidPermission.cs               # Name, MinSdkVersion, MaxSdkVersion
│   ├── AndroidFeature.cs                  # Name, Required
│   └── InfoPlistEntry.cs                  # Key, Type, Value
└── Tasks/
    ├── GenerateAndroidManifestTask.cs     # MSBuild task for AndroidManifestPermission/Feature
    ├── GenerateInfoPlistTask.cs           # MSBuild task for InfoPlistPermission
    └── GenerateMauiPermissionsTask.cs     # MSBuild task for MauiPermission
```

## Best Practices

1. **Prefer `MauiPermission`** over individual items — it handles all platform details and deduplication automatically
2. **Override default usage descriptions** before App Store / Play Store submission
3. **Combine permission sets freely** — deduplication ensures no duplicate manifest entries
4. **Use fine-grained items** only when you need SDK version constraints or custom permission names not covered by the known sets
5. **Check `obj/` output** to verify generated XML during development

## Example: Full MAUI Project Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>
        <OutputType>Exe</OutputType>
        <UseMaui>true</UseMaui>
    </PropertyGroup>

    <ItemGroup>
        <MauiPermission Include="Camera" />
        <MauiPermission Include="BluetoothLE" />
        <MauiPermission Include="Location" />
        <MauiPermission Include="Push" />
        <MauiPermission Include="Biometric" />
        <MauiPermission Include="Contacts" />
    </ItemGroup>
</Project>
```

This single declaration generates:
- **Android:** 12 `<uses-permission>` entries (deduplicated) + 2 `<uses-feature>` entries
- **iOS:** `UIBackgroundModes` array with `bluetooth-central`, `location`, `remote-notification` + 6 usage description strings
