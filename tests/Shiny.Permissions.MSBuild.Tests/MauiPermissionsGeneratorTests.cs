using Shiny.Permissions.MSBuild.Generators;

namespace Shiny.Permissions.MSBuild.Tests;

public class MauiPermissionsGeneratorTests
{
    [Fact]
    public Task SinglePermission_Camera()
    {
        var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(["Camera"]);
        return Verify(new { AndroidManifest = androidXml, InfoPlist = plistXml });
    }

    [Fact]
    public Task SinglePermission_LocationBackground()
    {
        var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(["LocationBackground"]);
        return Verify(new { AndroidManifest = androidXml, InfoPlist = plistXml });
    }

    [Fact]
    public Task SinglePermission_Push()
    {
        var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(["Push"]);
        return Verify(new { AndroidManifest = androidXml, InfoPlist = plistXml });
    }

    [Fact]
    public Task MultiplePermissions_MergesBackgroundModes()
    {
        var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(["Location", "Push"]);
        return Verify(new { AndroidManifest = androidXml, InfoPlist = plistXml });
    }

    [Fact]
    public Task MultiplePermissions_DeduplicatesEntries()
    {
        var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(["Location", "LocationBackground"]);
        return Verify(new { AndroidManifest = androidXml, InfoPlist = plistXml });
    }

    [Fact]
    public Task MultiplePermissions_BluetoothAndLocation()
    {
        var (androidXml, plistXml) = MauiPermissionsGenerator.Generate(["BluetoothLE", "Location"]);
        return Verify(new { AndroidManifest = androidXml, InfoPlist = plistXml });
    }

    [Fact]
    public void UnknownPermission_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => MauiPermissionsGenerator.Generate(["NonExistent"]));
        Assert.Contains("NonExistent", ex.Message);
        Assert.Contains("Known permissions", ex.Message);
    }

    [Fact]
    public void CaseInsensitive_PermissionNames()
    {
        var (xml1, plist1) = MauiPermissionsGenerator.Generate(["camera"]);
        var (xml2, plist2) = MauiPermissionsGenerator.Generate(["CAMERA"]);
        Assert.Equal(xml1, xml2);
        Assert.Equal(plist1, plist2);
    }

    [Fact]
    public void Resolve_DeduplicatesAndroidPermissions()
    {
        var (permissions, _, _) = MauiPermissionsGenerator.Resolve(["Location", "LocationBackground"]);
        var coarseCount = permissions.Count(p =>
            AndroidManifestGenerator.ResolvePermissionName(p.Name) == "android.permission.ACCESS_COARSE_LOCATION");
        Assert.Equal(1, coarseCount);
    }

    [Fact]
    public void Resolve_DeduplicatesAndroidFeatures()
    {
        var (_, features, _) = MauiPermissionsGenerator.Resolve(["Location", "LocationBackground"]);
        var gpsCount = features.Count(f =>
            AndroidManifestGenerator.ResolveFeatureName(f.Name) == "android.hardware.LOCATION.GPS");
        Assert.Equal(1, gpsCount);
    }

    [Fact]
    public void Resolve_MergesArrayPlistEntries()
    {
        var (_, _, plistEntries) = MauiPermissionsGenerator.Resolve(["Location", "Push"]);
        var bgModes = plistEntries.Single(e => e.Key == "UIBackgroundModes");
        Assert.Contains("location", bgModes.Value);
        Assert.Contains("remote-notification", bgModes.Value);
    }

    [Fact]
    public void Resolve_MergedArrayUsesStringArrayType()
    {
        var (_, _, plistEntries) = MauiPermissionsGenerator.Resolve(["Location", "Push"]);
        var bgModes = plistEntries.Single(e => e.Key == "UIBackgroundModes");
        Assert.Equal("stringarray", bgModes.Type);
    }

    [Fact]
    public void Resolve_MergesStringArrayPlistEntries()
    {
        var (_, _, plistEntries) = MauiPermissionsGenerator.Resolve(["BluetoothLE", "Location", "Push"]);
        var bgModes = plistEntries.Single(e => e.Key == "UIBackgroundModes");
        Assert.Contains("bluetooth-central", bgModes.Value);
        Assert.Contains("location", bgModes.Value);
        Assert.Contains("remote-notification", bgModes.Value);
    }

    [Fact]
    public void Resolve_DeduplicatesStringPlistEntries()
    {
        var (_, _, plistEntries) = MauiPermissionsGenerator.Resolve(["Location", "LocationBackground"]);
        var count = plistEntries.Count(e => e.Key == "NSLocationAlwaysUsageDescription");
        Assert.Equal(1, count);
    }
}
