# Shiny.Permissions.MSBuild API Reference

## MSBuild Item Groups

### MauiPermission

High-level permission declaration. Include value is the permission name.

| Include Value | Description |
|---|---|
| `BluetoothLE` | Bluetooth Low Energy scanning and connectivity |
| `Location` | GPS and network location |
| `LocationBackground` | Background location with foreground service |
| `Geofencing` | Background location for geofence monitoring |
| `Push` | Push / remote notifications |
| `Microphone` | Audio recording |
| `Contacts` | Read device contacts |
| `Calendar` | Read device calendar |
| `Camera` | Camera access |
| `Photos` | Photo library access |
| `Maps` | Map location access |
| `Biometric` | Face ID / fingerprint authentication |

No metadata is required. Permission name matching is case-insensitive.

### AndroidManifestPermission

Fine-grained Android `<uses-permission>` entry.

| Metadata | Required | Description |
|---|---|---|
| `Include` | Yes | Permission name (e.g. `CAMERA`, `ACCESS_FINE_LOCATION`) |
| `MinSdkVersion` | No | Minimum Android SDK version |
| `MaxSdkVersion` | No | Maximum Android SDK version |

Name resolution: names without 3+ dot-separated segments are prefixed with `android.permission.`.

### AndroidManifestFeature

Fine-grained Android `<uses-feature>` entry.

| Metadata | Required | Description |
|---|---|---|
| `Include` | Yes | Feature name (e.g. `CAMERA`, `LOCATION.GPS`) |
| `Required` | No | `true` or `false` |

Name resolution: names without 3+ dot-separated segments are prefixed with `android.hardware.`.

### InfoPlistPermission

Fine-grained iOS Info.plist key/value entry.

| Metadata | Required | Default | Description |
|---|---|---|---|
| `Include` | Yes | — | Plist key (e.g. `NSCameraUsageDescription`) |
| `Type` | No | `string` | `string`, `boolean` / `bool`, or `array` |
| `Value` | No | empty | Value. For arrays, semicolon-delimited (e.g. `fb;instagram;twitter`) |

## Generator Classes

### MauiPermissionsGenerator

**Namespace:** `Shiny.Permissions.MSBuild.Generators`

Static class that resolves permission names to platform-specific entries.

```csharp
// Known permission names
IReadOnlyList<string> KnownPermissions { get; }

// Check if a name is a known permission
bool IsKnownPermission(string name);

// Resolve permission names to platform entries
(List<AndroidPermission>, List<AndroidFeature>, List<InfoPlistEntry>) Resolve(
    IEnumerable<string> permissionNames);

// Resolve with additional custom plist entries
(List<AndroidPermission>, List<AndroidFeature>, List<InfoPlistEntry>) Resolve(
    IEnumerable<string> permissionNames,
    IEnumerable<InfoPlistEntry> additionalPlistEntries);

// Generate final XML strings
(string AndroidManifestXml, string InfoPlistXml) Generate(
    IEnumerable<string> permissionNames);

// Generate with additional custom plist entries
(string AndroidManifestXml, string InfoPlistXml) Generate(
    IEnumerable<string> permissionNames,
    IEnumerable<InfoPlistEntry> additionalPlistEntries);
```

### AndroidManifestGenerator

**Namespace:** `Shiny.Permissions.MSBuild.Generators`

Static class that generates Android manifest XML.

```csharp
// Resolve a short permission name to fully qualified
string ResolvePermissionName(string name);

// Resolve a short feature name to fully qualified
string ResolveFeatureName(string name);

// Generate manifest XML from permissions and features
string GenerateManifestXml(
    IEnumerable<AndroidPermission> permissions,
    IEnumerable<AndroidFeature> features);
```

### InfoPlistGenerator

**Namespace:** `Shiny.Permissions.MSBuild.Generators`

Static class that generates iOS Info.plist XML.

```csharp
// Generate plist XML from entries
string GeneratePlistXml(IEnumerable<InfoPlistEntry> entries);
```

## Models

### AndroidPermission

```csharp
public class AndroidPermission
{
    public string Name { get; set; }           // e.g. "CAMERA" or "android.permission.CAMERA"
    public string? MinSdkVersion { get; set; } // e.g. "19"
    public string? MaxSdkVersion { get; set; } // e.g. "32"
}
```

### AndroidFeature

```csharp
public class AndroidFeature
{
    public string Name { get; set; }      // e.g. "LOCATION.GPS"
    public string? Required { get; set; } // "true" or "false"
}
```

### InfoPlistEntry

```csharp
public class InfoPlistEntry
{
    public string Key { get; set; }   // e.g. "NSCameraUsageDescription"
    public string Type { get; set; }  // "string", "boolean", "bool", or "array"
    public string Value { get; set; } // e.g. "We need camera access" or "fb;instagram"
}
```

## MSBuild Tasks

### GenerateMauiPermissionsTask

Processes `MauiPermission` items and generates both output files.

| Property | Type | Required | Description |
|---|---|---|---|
| `Permissions` | `ITaskItem[]` | Yes | The `MauiPermission` items |
| `AndroidManifestOutputPath` | `string` | Yes | Path to write AndroidManifest.xml |
| `InfoPlistOutputPath` | `string` | Yes | Path to write Info.plist |

### GenerateAndroidManifestTask

Processes `AndroidManifestPermission` and `AndroidManifestFeature` items.

| Property | Type | Required | Description |
|---|---|---|---|
| `Permissions` | `ITaskItem[]` | Yes | The `AndroidManifestPermission` items |
| `Features` | `ITaskItem[]` | Yes | The `AndroidManifestFeature` items |
| `OutputPath` | `string` | Yes | Path to write AndroidManifest.xml |

### GenerateInfoPlistTask

Processes `InfoPlistPermission` items.

| Property | Type | Required | Description |
|---|---|---|---|
| `Entries` | `ITaskItem[]` | Yes | The `InfoPlistPermission` items |
| `OutputPath` | `string` | Yes | Path to write Info.plist |
