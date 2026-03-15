using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class AndroidManifestFeatureGeneratorTests
{
    public class ResolveNameTests
    {
        [Theory]
        [InlineData("android.hardware.LOCATION.GPS", "android.hardware.LOCATION.GPS")]
        [InlineData("some.namedfeature.HELLO", "some.namedfeature.HELLO")]
        [InlineData("com.example.custom.FEATURE", "com.example.custom.FEATURE")]
        public void FullyQualified_ThreeOrMoreSegments_ReturnedAsIs(string input, string expected)
        {
            Assert.Equal(expected, AndroidManifestFeatureGenerator.ResolveName(input));
        }

        [Theory]
        [InlineData("CAMERA", "android.hardware.CAMERA")]
        [InlineData("BLUETOOTH", "android.hardware.BLUETOOTH")]
        public void SingleSegment_PrefixedWithAndroidHardware(string input, string expected)
        {
            Assert.Equal(expected, AndroidManifestFeatureGenerator.ResolveName(input));
        }

        [Theory]
        [InlineData("LOCATION.NETWORK", "android.hardware.LOCATION.NETWORK")]
        [InlineData("LOCATION.GPS", "android.hardware.LOCATION.GPS")]
        [InlineData("a.b", "android.hardware.a.b")]
        public void TwoSegments_StillPrefixed(string input, string expected)
        {
            Assert.Equal(expected, AndroidManifestFeatureGenerator.ResolveName(input));
        }

        [Fact]
        public void EmptyString_PrefixedWithAndroidHardware()
        {
            Assert.Equal("android.hardware.", AndroidManifestFeatureGenerator.ResolveName(""));
        }
    }

    public class GenerateElementTests
    {
        [Fact]
        public void WithRequiredTrue()
        {
            var feature = new AndroidFeature
            {
                Name = "android.hardware.LOCATION.GPS",
                Required = "true"
            };
            var xml = AndroidManifestFeatureGenerator.GenerateElement(feature);

            Assert.Equal(
                "<uses-feature android:name=\"android.hardware.LOCATION.GPS\" android:required=\"true\" />",
                xml
            );
        }

        [Fact]
        public void WithRequiredFalse()
        {
            var feature = new AndroidFeature
            {
                Name = "LOCATION.NETWORK",
                Required = "false"
            };
            var xml = AndroidManifestFeatureGenerator.GenerateElement(feature);

            Assert.Equal(
                "<uses-feature android:name=\"android.hardware.LOCATION.NETWORK\" android:required=\"false\" />",
                xml
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NullOrWhitespace_Required_OmittedFromOutput(string? value)
        {
            var feature = new AndroidFeature
            {
                Name = "CAMERA",
                Required = value
            };
            var xml = AndroidManifestFeatureGenerator.GenerateElement(feature);

            Assert.DoesNotContain("android:required", xml);
            Assert.Equal(
                "<uses-feature android:name=\"android.hardware.CAMERA\" />",
                xml
            );
        }

        [Fact]
        public void NameResolution_AppliedInElement()
        {
            var feature = new AndroidFeature { Name = "CAMERA", Required = "true" };
            var xml = AndroidManifestFeatureGenerator.GenerateElement(feature);

            Assert.Contains("android:name=\"android.hardware.CAMERA\"", xml);
        }
    }

    public class GenerateManifestXmlTests
    {
        [Fact]
        public void MatchesSpecExample()
        {
            var features = new[]
            {
                new AndroidFeature { Name = "android.hardware.LOCATION.GPS", Required = "true" },
                new AndroidFeature { Name = "LOCATION.NETWORK", Required = "false" },
                new AndroidFeature { Name = "some.namedfeature.HELLO", Required = "true" },
            };

            var xml = AndroidManifestFeatureGenerator.GenerateManifestXml(features);

            Assert.Contains("<manifest>", xml);
            Assert.Contains("</manifest>", xml);
            Assert.Contains("<uses-feature android:name=\"android.hardware.LOCATION.GPS\" android:required=\"true\" />", xml);
            Assert.Contains("<uses-feature android:name=\"android.hardware.LOCATION.NETWORK\" android:required=\"false\" />", xml);
            Assert.Contains("<uses-feature android:name=\"some.namedfeature.HELLO\" android:required=\"true\" />", xml);
        }

        [Fact]
        public void EmptyCollection_ValidManifestShell()
        {
            var xml = AndroidManifestFeatureGenerator.GenerateManifestXml([]);

            Assert.Contains("<manifest>", xml);
            Assert.Contains("</manifest>", xml);
            Assert.DoesNotContain("<uses-feature", xml);
        }

        [Fact]
        public void SingleFeature_ValidOutput()
        {
            var features = new[]
            {
                new AndroidFeature { Name = "CAMERA", Required = "true" },
            };

            var xml = AndroidManifestFeatureGenerator.GenerateManifestXml(features);

            Assert.Contains("<uses-feature android:name=\"android.hardware.CAMERA\" android:required=\"true\" />", xml);
        }

        [Fact]
        public void Elements_AreIndentedWithFourSpaces()
        {
            var features = new[]
            {
                new AndroidFeature { Name = "CAMERA", Required = "true" },
            };

            var xml = AndroidManifestFeatureGenerator.GenerateManifestXml(features);
            var lines = xml.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var featureLine = lines.First(l => l.Contains("<uses-feature"));

            Assert.StartsWith("    <uses-feature", featureLine);
        }
    }
}
