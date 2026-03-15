# Shiny Permissions MSBuild

An MSBuild task library that automatically generates Android manifest permissions/features and iOS Info.plist entries during the build process. Declare permissions in your project file and the correct platform-specific XML is generated for you.

## Installation

Install the NuGet package:

```
dotnet add package Shiny.Permissions.MSBuild
```

Once installed, the `.props` and `.targets` files are imported automatically. Generation runs before the `Build` target and only when items are defined.

## Android Manifest Permissions

Add `AndroidManifestPermission` items to your project file:

```xml
<ItemGroup>
  <AndroidManifestPermission Include="CAMERA" />
  <AndroidManifestPermission Include="ACCESS_FINE_LOCATION" />
  <AndroidManifestPermission Include="READ_EXTERNAL_STORAGE" MaxSdkVersion="32" />
  <AndroidManifestPermission Include="WRITE_EXTERNAL_STORAGE" MinSdkVersion="19" MaxSdkVersion="28" />
</ItemGroup>
```

This generates `$(IntermediateOutputPath)AndroidManifestPermissions.xml`:

```xml
<manifest>
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

This generates `$(IntermediateOutputPath)AndroidManifestFeatures.xml`:

```xml
<manifest>
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

  <!-- Array values (semicolon-delimited) -->
  <InfoPlistPermission Include="LSApplicationQueriesSchemes" Type="array" Value="fb;instagram;twitter" />
</ItemGroup>
```

This generates `$(IntermediateOutputPath)InfoPlist.xml`:

```xml
<plist>
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
    </dict>
</plist>
```

### Metadata

| Metadata | Required | Default | Description |
|---|---|---|---|
| `Type` | No | `string` | Value type: `string`, `boolean` / `bool`, or `array` |
| `Value` | No | empty | The entry value. For arrays, separate items with `;` |

## Output

All generated XML files are written to `$(IntermediateOutputPath)` (typically `obj/<Configuration>/`). Each target only runs when its corresponding item group contains at least one item.

| Item Group | Output File |
|---|---|
| `AndroidManifestPermission` | `AndroidManifestPermissions.xml` |
| `AndroidManifestFeature` | `AndroidManifestFeatures.xml` |
| `InfoPlistPermission` | `InfoPlist.xml` |




TODO

## TODO
- Duplicate keys in Info.plist, AndroidManifest.xml error
- If AndroidManifestPermission/AndroidManifestFeature file creation collision
- Don't generate empty files
- Generate properly formatted manifests & plists with XML declaration, root elements, etc.

## MAUI PERMISSIONS:
- BluetoothLE
- Location (not both)
- LocationBackground (not both)
- Microphone
- Calendar
- Camera
- MediaPicker
- Maps
- Biometric

### GIVEN:
csproj
```xml
<ItemGroup>
    <MauiPermission Include="BluetoothLE" />
</ItemGroup>
```

### GENERATED FILES:

AndroidManifest.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.BATTERY_STATS" />
    <uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED"/>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />      

    <uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    <uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
    <uses-permission android:name="android.permission.FOREGROUND_SERVICE_LOCATION" />

    <uses-feature android:name="android.hardware.LOCATION.GPS" android:required="false" />
    <uses-feature android:name="android.hardware.LOCATION.NETWORK" android:required="false" />
</manifest>
```

Info.plist
```xml
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
  <plist version="1.0">
  <dict>
  
        <key>NSLocationAlwaysUsageDescription</key>
        <string>Say something useful here that your users will understand</string>
        
        <key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
        <string>Say something useful here that your users will understand</string>
        
        <key>NSLocationWhenInUseUsageDescription</key>
        <string>Say something useful here that your users will understand</string>
        
        <key>UIBackgroundModes</key>
        <array>
        
            <string>location</string>
            
        </array>
          
</dict>
</plist>
```