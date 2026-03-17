# Shiny Permissions MSBuild

An MSBuild task library that automatically generates Android manifest permissions/features and iOS Info.plist entries during the build process. Declare permissions in your project file and the correct platform-specific XML is generated for you.

## Installation

Install the NuGet package:

```
dotnet add package Shiny.Permissions.MSBuild
```

Once installed, the `.props` and `.targets` files are imported automatically. Generation runs before the `Build` target and only when items are defined.

## MAUI Permissions

The simplest way to declare permissions for a .NET MAUI app. Add `MauiPermission` items and the task generates both Android manifest entries and iOS Info.plist entries automatically:

```xml
<ItemGroup>
    <MauiPermission Include="Camera" />
    <MauiPermission Include="BluetoothLE" />
    <MauiPermission Include="Location" />
    <MauiPermission Include="Push" />
    <MauiPermission Include="Biometric" />
    <MauiPermission Include="Contacts" />
</ItemGroup>
```

### Known Permission Sets

| Permission | Android Permissions | iOS Entries |
|---|---|---|
| `BluetoothLE` | `BLUETOOTH`, `BLUETOOTH_ADMIN`, `BLUETOOTH_CONNECT`, `BLUETOOTH_SCAN`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION` | `UIBackgroundModes` (bluetooth-central), `NSBluetoothAlwaysUsageDescription` |
| `Location` | `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION` + features: `LOCATION.GPS`, `LOCATION.NETWORK` | `UIBackgroundModes` (location), `NSLocationAlwaysUsageDescription`, `NSLocationWhenInUseUsageDescription`, `NSLocationAlwaysAndWhenInUseUsageDescription` |
| `LocationBackground` | `FOREGROUND_SERVICE_LOCATION`, `FOREGROUND_SERVICE`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION` + features: `LOCATION.GPS`, `LOCATION.NETWORK` | `UIBackgroundModes` (location), `NSLocationAlwaysUsageDescription`, `NSLocationWhenInUseUsageDescription`, `NSLocationAlwaysAndWhenInUseUsageDescription` |
| `Geofencing` | `ACCESS_BACKGROUND_LOCATION`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION` + features: `LOCATION.GPS`, `LOCATION.NETWORK` | `UIBackgroundModes` (location), `NSLocationAlwaysUsageDescription`, `NSLocationWhenInUseUsageDescription`, `NSLocationAlwaysAndWhenInUseUsageDescription` |
| `Push` | `POST_NOTIFICATIONS` | `UIBackgroundModes` (remote-notification) |
| `Microphone` | `RECORD_AUDIO` | `NSMicrophoneUsageDescription` |
| `Contacts` | `READ_CONTACTS` | `NSContactsUsageDescription` |
| `Calendar` | `READ_CALENDAR` | `NSCalendarsUsageDescription` |
| `Camera` | `CAMERA` | `NSCameraUsageDescription` |
| `Photos` | `READ_EXTERNAL_STORAGE` | `NSPhotoLibraryUsageDescription` |
| `Maps` | `ACCESS_FINE_LOCATION` | `NSLocationWhenInUseUsageDescription` |
| `Biometric` | `USE_BIOMETRIC` | `NSFaceIDUsageDescription` |

### Deduplication and Merging

When multiple permission sets share entries, the generator automatically:

- **Deduplicates** Android permissions and features (e.g. `BluetoothLE` + `Location` both declare `ACCESS_FINE_LOCATION` — it appears only once)
- **Merges** iOS array entries (e.g. `BluetoothLE` + `Location` + `Push` merge into a single `UIBackgroundModes` array with `bluetooth-central`, `location`, and `remote-notification`)

### Output

`MauiPermission` items generate two files in `$(IntermediateOutputPath)`:

| File | Description |
|---|---|
| `AndroidManifest.xml` | Android permissions and features |
| `Info.plist` | iOS Info.plist entries |

## Android Manifest Permissions

For fine-grained control, add `AndroidManifestPermission` items directly:

```xml
<ItemGroup>
  <AndroidManifestPermission Include="CAMERA" />
  <AndroidManifestPermission Include="ACCESS_FINE_LOCATION" />
  <AndroidManifestPermission Include="READ_EXTERNAL_STORAGE" MaxSdkVersion="32" />
  <AndroidManifestPermission Include="WRITE_EXTERNAL_STORAGE" MinSdkVersion="19" MaxSdkVersion="28" />
</ItemGroup>
```

This generates `$(IntermediateOutputPath)AndroidManifest.xml`:

```xml
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" android:maxSdkVersion="32" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" android:minSdkVersion="19" android:maxSdkVersion="28" />
</manifest>
```

### Metadata

| Metadata | Required | Description |
|---|---|---|
| `MinSdkVersion` | No | Minimum Android SDK version for this permission |
| `MaxSdkVersion` | No | Maximum Android SDK version for this permission |

### Name Resolution

Short names like `CAMERA` are automatically prefixed with `android.permission.`. Fully qualified names with 3+ dot-separated segments (e.g. `com.example.myapp.CUSTOM_PERMISSION`) are used as-is.

## Android Manifest Features

Add `AndroidManifestFeature` items to your project file:

```xml
<ItemGroup>
  <AndroidManifestFeature Include="CAMERA" />
  <AndroidManifestFeature Include="LOCATION.GPS" Required="true" />
  <AndroidManifestFeature Include="BLUETOOTH" Required="false" />
</ItemGroup>
```

This generates `$(IntermediateOutputPath)AndroidManifest.xml`:

```xml
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-feature android:name="android.hardware.CAMERA" />
    <uses-feature android:name="android.hardware.LOCATION.GPS" android:required="true" />
    <uses-feature android:name="android.hardware.BLUETOOTH" android:required="false" />
</manifest>
```

### Metadata

| Metadata | Required | Description |
|---|---|---|
| `Required` | No | Whether the feature is required (`true` or `false`) |

### Name Resolution

Short names like `CAMERA` are automatically prefixed with `android.hardware.`. Fully qualified names with 3+ dot-separated segments are used as-is.

## iOS Info.plist Entries

Add `InfoPlistPermission` items to your project file:

```xml
<ItemGroup>
  <!-- String values -->
  <InfoPlistPermission Include="NSCameraUsageDescription" Type="string" Value="We need camera access for video calls" />
  <InfoPlistPermission Include="NSLocationWhenInUseUsageDescription" Type="string" Value="Your location helps us find nearby places" />

  <!-- Boolean values -->
  <InfoPlistPermission Include="UIRequiresPersistentWiFi" Type="boolean" Value="true" />

  <!-- String array values (semicolon-delimited) -->
  <InfoPlistPermission Include="LSApplicationQueriesSchemes" Type="stringarray" Value="fb;instagram;twitter" />

  <!-- Integer array values (semicolon-delimited) -->
  <InfoPlistPermission Include="UIDeviceFamily" Type="integerarray" Value="1;2" />
</ItemGroup>
```

This generates `$(IntermediateOutputPath)Info.plist`:

```xml
<plist version="1.0">
    <dict>
        <key>NSCameraUsageDescription</key>
        <string>We need camera access for video calls</string>
        <key>NSLocationWhenInUseUsageDescription</key>
        <string>Your location helps us find nearby places</string>
        <key>UIRequiresPersistentWiFi</key>
        <true/>
        <key>LSApplicationQueriesSchemes</key>
        <array>
            <string>fb</string>
            <string>instagram</string>
            <string>twitter</string>
        </array>
        <key>UIDeviceFamily</key>
        <array>
            <integer>1</integer>
            <integer>2</integer>
        </array>
    </dict>
</plist>
```

### Metadata

| Metadata | Required | Default | Description |
|---|---|---|---|
| `Type` | No | `string` | Value type: `string`, `boolean` / `bool`, `stringarray`, or `integerarray` |
| `Value` | No | empty | The entry value. For arrays, separate items with `;` |

## Samples

See the [`samples/SampleMauiApp`](samples/SampleMauiApp) project for a .NET MAUI app demonstrating `MauiPermission` with Camera, BluetoothLE, Location, Push, Biometric, and Contacts.