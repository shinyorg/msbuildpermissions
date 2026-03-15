using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class AndroidManifestFeatureGeneratorTests
{
    [Fact]
    public Task SingleShortName()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "CAMERA", Required = "true" },
        };
        return Verify(AndroidManifestFeatureGenerator.GenerateManifestXml(features));
    }

    [Fact]
    public Task SingleFullyQualifiedName()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "android.hardware.LOCATION.GPS", Required = "true" },
        };
        return Verify(AndroidManifestFeatureGenerator.GenerateManifestXml(features));
    }

    [Fact]
    public Task RequiredFalse()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
        };
        return Verify(AndroidManifestFeatureGenerator.GenerateManifestXml(features));
    }

    [Fact]
    public Task RequiredOmitted()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "CAMERA" },
        };
        return Verify(AndroidManifestFeatureGenerator.GenerateManifestXml(features));
    }

    [Fact]
    public Task MultipleFeatures_MixedNames()
    {
        var features = new[]
        {
            new AndroidFeature { Name = "android.hardware.LOCATION.GPS", Required = "true" },
            new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
            new AndroidFeature { Name = "some.namedfeature.HELLO", Required = "true" },
            new AndroidFeature { Name = "CAMERA" },
        };
        return Verify(AndroidManifestFeatureGenerator.GenerateManifestXml(features));
    }
}
