using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class AndroidManifestGeneratorTests
{
    [Fact]
    public Task PermissionsOnly()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA" },
            new AndroidPermission { Name = "WRITE_EXTERNAL_STORAGE" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml(permissions, []));
    }

    [Fact]
    public Task FeaturesOnly()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "CAMERA", Required = "true" },
            new AndroidFeature { Name = "LOCATION.GPS", Required = "false" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml([], features));
    }

    [Fact]
    public Task PermissionsAndFeatures()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "android.permission.READ_EXTERNAL_STORAGE", MaxSdkVersion = "28" },
            new AndroidPermission { Name = "WRITE_EXTERNAL_STORAGE" },
            new AndroidPermission { Name = "CAMERA", MinSdkVersion = "21", MaxSdkVersion = "33" },
        };
        var features = new[]
        {
            new AndroidFeature { Name = "android.hardware.LOCATION.GPS", Required = "true" },
            new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
            new AndroidFeature { Name = "CAMERA" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml(permissions, features));
    }

    [Fact]
    public Task FullyQualifiedPermissionName()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "some.namedpermissions.ACCESS" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml(permissions, []));
    }

    [Fact]
    public Task FullyQualifiedFeatureName()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "some.namedfeature.HELLO", Required = "true" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml([], features));
    }

    [Fact]
    public Task PermissionWithMinSdkVersion()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA", MinSdkVersion = "21" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml(permissions, []));
    }

    [Fact]
    public Task FeatureRequiredOmitted()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "CAMERA" },
        };
        return Verify(AndroidManifestGenerator.GenerateManifestXml([], features));
    }

    [Fact]
    public void DuplicatePermission_ShortNames()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA" },
            new AndroidPermission { Name = "CAMERA" },
        };
        var ex = Assert.Throws<InvalidOperationException>(
            () => AndroidManifestGenerator.GenerateManifestXml(permissions, []));
        Assert.Contains("android.permission.CAMERA", ex.Message);
    }

    [Fact]
    public void DuplicatePermission_ShortAndFullyQualified()
    {
        var permissions = new[]
        {
            new AndroidPermission { Name = "CAMERA" },
            new AndroidPermission { Name = "android.permission.CAMERA" },
        };
        var ex = Assert.Throws<InvalidOperationException>(
            () => AndroidManifestGenerator.GenerateManifestXml(permissions, []));
        Assert.Contains("android.permission.CAMERA", ex.Message);
        Assert.Contains("CAMERA", ex.Message);
    }

    [Fact]
    public void DuplicateFeature_ShortNames()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "CAMERA", Required = "true" },
            new AndroidFeature { Name = "CAMERA", Required = "false" },
        };
        var ex = Assert.Throws<InvalidOperationException>(
            () => AndroidManifestGenerator.GenerateManifestXml([], features));
        Assert.Contains("android.hardware.CAMERA", ex.Message);
    }

    [Fact]
    public void DuplicateFeature_ShortAndFullyQualified()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "CAMERA" },
            new AndroidFeature { Name = "android.hardware.CAMERA" },
        };
        var ex = Assert.Throws<InvalidOperationException>(
            () => AndroidManifestGenerator.GenerateManifestXml([], features));
        Assert.Contains("android.hardware.CAMERA", ex.Message);
    }
}
