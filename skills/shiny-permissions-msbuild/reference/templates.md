# Shiny.Permissions.MSBuild Templates

## MAUI Project with Common Permissions

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>
        <OutputType>Exe</OutputType>
        <UseMaui>true</UseMaui>
        <SingleProject>true</SingleProject>
    </PropertyGroup>

    <ItemGroup>
        <MauiPermission Include="Camera" />
        <MauiPermission Include="Location" />
        <MauiPermission Include="Push" />
    </ItemGroup>
</Project>
```

## Bluetooth + Location App

```xml
<ItemGroup>
    <MauiPermission Include="BluetoothLE" />
    <MauiPermission Include="Location" />
</ItemGroup>
```

Generates merged `UIBackgroundModes` with both `bluetooth-central` and `location`.
Android permissions are deduplicated — `ACCESS_COARSE_LOCATION` and `ACCESS_FINE_LOCATION` appear once even though both sets include them.

## Background Location with Geofencing

```xml
<ItemGroup>
    <MauiPermission Include="LocationBackground" />
    <MauiPermission Include="Geofencing" />
</ItemGroup>
```

Generates `FOREGROUND_SERVICE`, `FOREGROUND_SERVICE_LOCATION`, `ACCESS_BACKGROUND_LOCATION`, `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION` on Android.

## Fine-Grained Android Permissions with SDK Constraints

```xml
<ItemGroup>
    <AndroidManifestPermission Include="READ_EXTERNAL_STORAGE" MaxSdkVersion="32" />
    <AndroidManifestPermission Include="READ_MEDIA_IMAGES" MinSdkVersion="33" />
    <AndroidManifestPermission Include="READ_MEDIA_VIDEO" MinSdkVersion="33" />
</ItemGroup>
```

## Custom iOS Info.plist Entries

```xml
<ItemGroup>
    <!-- Usage description string -->
    <InfoPlistPermission Include="NSCameraUsageDescription"
                         Type="string"
                         Value="We use the camera for scanning QR codes" />

    <!-- Boolean -->
    <InfoPlistPermission Include="UIRequiresPersistentWiFi"
                         Type="boolean"
                         Value="true" />

    <!-- String array (semicolon-delimited) -->
    <InfoPlistPermission Include="LSApplicationQueriesSchemes"
                         Type="stringarray"
                         Value="fb;instagram;twitter" />

    <!-- Integer array (semicolon-delimited) -->
    <InfoPlistPermission Include="UIDeviceFamily"
                         Type="integerarray"
                         Value="1;2" />
</ItemGroup>
```

## Mixed: MauiPermission with Fine-Grained Overrides

Use `MauiPermission` for the high-level sets, then add individual items for extras not covered by the known sets:

```xml
<ItemGroup>
    <!-- High-level sets -->
    <MauiPermission Include="Camera" />
    <MauiPermission Include="Location" />

    <!-- Additional Android permission not in any known set -->
    <AndroidManifestPermission Include="VIBRATE" />

    <!-- Additional iOS entry not in any known set -->
    <InfoPlistPermission Include="NSAppleMusicUsageDescription"
                         Type="string"
                         Value="We play music during workouts" />
</ItemGroup>
```

> **Note:** `MauiPermission` and the fine-grained item types use separate MSBuild targets. Both can coexist in the same project.
