using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class AndroidManifestPermissionGeneratorTests
{
    [Fact]
    public Task SingleShortName()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA" },
        };
        return Verify(AndroidManifestPermissionGenerator.GenerateManifestXml(permissions));
    }

    [Fact]
    public Task SingleFullyQualifiedName()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "android.permission.READ_EXTERNAL_STORAGE" },
        };
        return Verify(AndroidManifestPermissionGenerator.GenerateManifestXml(permissions));
    }

    [Fact]
    public Task WithMaxSdkVersion()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "READ_EXTERNAL_STORAGE", MaxSdkVersion = "28" },
        };
        return Verify(AndroidManifestPermissionGenerator.GenerateManifestXml(permissions));
    }

    [Fact]
    public Task WithMinSdkVersion()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA", MinSdkVersion = "21" },
        };
        return Verify(AndroidManifestPermissionGenerator.GenerateManifestXml(permissions));
    }

    [Fact]
    public Task WithBothSdkVersions()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA", MinSdkVersion = "21", MaxSdkVersion = "33" },
        };
        return Verify(AndroidManifestPermissionGenerator.GenerateManifestXml(permissions));
    }

    [Fact]
    public Task MultiplePermissions_MixedNames()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "android.permission.READ_EXTERNAL_STORAGE", MaxSdkVersion = "28" },
            new AndroidPermission { Name = "WRITE_EXTERNAL_STORAGE" },
            new AndroidPermission { Name = "some.namedpermissions.ACCESS" },
            new AndroidPermission { Name = "CAMERA", MinSdkVersion = "21", MaxSdkVersion = "33" },
        };
        return Verify(AndroidManifestPermissionGenerator.GenerateManifestXml(permissions));
    }
}
