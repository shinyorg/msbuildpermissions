using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class AndroidManifestPermissionGeneratorTests
{
    public class ResolveNameTests
    {
        [Theory]
        [InlineData("android.permission.READ_EXTERNAL_STORAGE", "android.permission.READ_EXTERNAL_STORAGE")]
        [InlineData("some.namedpermissions.ACCESS", "some.namedpermissions.ACCESS")]
        [InlineData("com.example.myapp.CUSTOM", "com.example.myapp.CUSTOM")]
        public void FullyQualified_ThreeOrMoreSegments_ReturnedAsIs(string input, string expected)
        {
            Assert.Equal(expected, AndroidManifestPermissionGenerator.ResolveName(input));
        }

        [Theory]
        [InlineData("WRITE_EXTERNAL_STORAGE", "android.permission.WRITE_EXTERNAL_STORAGE")]
        [InlineData("CAMERA", "android.permission.CAMERA")]
        [InlineData("ACCESS_FINE_LOCATION", "android.permission.ACCESS_FINE_LOCATION")]
        public void SingleSegment_PrefixedWithAndroidPermission(string input, string expected)
        {
            Assert.Equal(expected, AndroidManifestPermissionGenerator.ResolveName(input));
        }

        [Theory]
        [InlineData("partial.NAME", "android.permission.partial.NAME")]
        [InlineData("a.b", "android.permission.a.b")]
        public void TwoSegments_StillPrefixed(string input, string expected)
        {
            Assert.Equal(expected, AndroidManifestPermissionGenerator.ResolveName(input));
        }

        [Fact]
        public void EmptyString_PrefixedWithAndroidPermission()
        {
            Assert.Equal("android.permission.", AndroidManifestPermissionGenerator.ResolveName(""));
        }
    }

    public class GenerateElementTests
    {
        [Fact]
        public void BasicPermission_NoOptionalAttributes()
        {
            var perm = new AndroidPermission { Name = "WRITE_EXTERNAL_STORAGE" };
            var xml = AndroidManifestPermissionGenerator.GenerateElement(perm);

            Assert.Equal(
                "<uses-permission android:name=\"android.permission.WRITE_EXTERNAL_STORAGE\" />",
                xml
            );
        }

        [Fact]
        public void WithMaxSdkVersion()
        {
            var perm = new AndroidPermission
            {
                Name = "android.permission.READ_EXTERNAL_STORAGE",
                MaxSdkVersion = "28"
            };
            var xml = AndroidManifestPermissionGenerator.GenerateElement(perm);

            Assert.Contains("android:maxSdkVersion=\"28\"", xml);
            Assert.StartsWith("<uses-permission", xml);
            Assert.EndsWith("/>", xml);
        }

        [Fact]
        public void WithMinSdkVersion()
        {
            var perm = new AndroidPermission
            {
                Name = "CAMERA",
                MinSdkVersion = "21"
            };
            var xml = AndroidManifestPermissionGenerator.GenerateElement(perm);

            Assert.Contains("android:minSdkVersion=\"21\"", xml);
        }

        [Fact]
        public void WithBothSdkVersions()
        {
            var perm = new AndroidPermission
            {
                Name = "CAMERA",
                MinSdkVersion = "21",
                MaxSdkVersion = "33"
            };
            var xml = AndroidManifestPermissionGenerator.GenerateElement(perm);

            Assert.Contains("android:minSdkVersion=\"21\"", xml);
            Assert.Contains("android:maxSdkVersion=\"33\"", xml);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NullOrWhitespace_SdkVersions_OmittedFromOutput(string? value)
        {
            var perm = new AndroidPermission
            {
                Name = "CAMERA",
                MinSdkVersion = value,
                MaxSdkVersion = value
            };
            var xml = AndroidManifestPermissionGenerator.GenerateElement(perm);

            Assert.DoesNotContain("minSdkVersion", xml);
            Assert.DoesNotContain("maxSdkVersion", xml);
        }

        [Fact]
        public void AttributeOrder_NameThenMinThenMax()
        {
            var perm = new AndroidPermission
            {
                Name = "CAMERA",
                MinSdkVersion = "21",
                MaxSdkVersion = "33"
            };
            var xml = AndroidManifestPermissionGenerator.GenerateElement(perm);

            var nameIdx = xml.IndexOf("android:name");
            var minIdx = xml.IndexOf("android:minSdkVersion");
            var maxIdx = xml.IndexOf("android:maxSdkVersion");

            Assert.True(nameIdx < minIdx);
            Assert.True(minIdx < maxIdx);
        }
    }

    public class GenerateManifestXmlTests
    {
        [Fact]
        public void MatchesSpecExample()
        {
            var permissions = new[]
            {
                new AndroidPermission { Name = "android.permission.READ_EXTERNAL_STORAGE", MaxSdkVersion = "28" },
                new AndroidPermission { Name = "WRITE_EXTERNAL_STORAGE" },
                new AndroidPermission { Name = "some.namedpermissions.ACCESS" },
            };

            var xml = AndroidManifestPermissionGenerator.GenerateManifestXml(permissions);

            Assert.Contains("<manifest>", xml);
            Assert.Contains("</manifest>", xml);
            Assert.Contains("<uses-permission android:name=\"android.permission.READ_EXTERNAL_STORAGE\" android:maxSdkVersion=\"28\" />", xml);
            Assert.Contains("<uses-permission android:name=\"android.permission.WRITE_EXTERNAL_STORAGE\" />", xml);
            Assert.Contains("<uses-permission android:name=\"some.namedpermissions.ACCESS\" />", xml);
        }

        [Fact]
        public void EmptyCollection_ValidManifestShell()
        {
            var xml = AndroidManifestPermissionGenerator.GenerateManifestXml([]);

            Assert.Contains("<manifest>", xml);
            Assert.Contains("</manifest>", xml);
            Assert.DoesNotContain("<uses-permission", xml);
        }

        [Fact]
        public void SinglePermission_ValidOutput()
        {
            var permissions = new[]
            {
                new AndroidPermission { Name = "CAMERA" },
            };

            var xml = AndroidManifestPermissionGenerator.GenerateManifestXml(permissions);

            Assert.Contains("<uses-permission android:name=\"android.permission.CAMERA\" />", xml);
        }

        [Fact]
        public void Elements_AreIndentedWithFourSpaces()
        {
            var permissions = new[]
            {
                new AndroidPermission { Name = "CAMERA" },
            };

            var xml = AndroidManifestPermissionGenerator.GenerateManifestXml(permissions);
            var lines = xml.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var permLine = lines.First(l => l.Contains("<uses-permission"));

            Assert.StartsWith("    <uses-permission", permLine);
        }
    }
}
