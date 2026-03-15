using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class InfoPlistGeneratorTests
{
    public class StringTypeTests
    {
        [Fact]
        public void GeneratesKeyAndStringElement()
        {
            var entry = new InfoPlistEntry
            {
                Key = "NSAppleMusicUsageDescription",
                Type = "string",
                Value = "TuneGames needs access to your music library to play music trivia games"
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<key>NSAppleMusicUsageDescription</key>", xml);
            Assert.Contains("<string>TuneGames needs access to your music library to play music trivia games</string>", xml);
        }

        [Fact]
        public void EmptyValue_GeneratesEmptyStringElement()
        {
            var entry = new InfoPlistEntry
            {
                Key = "SomeKey",
                Type = "string",
                Value = ""
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<string></string>", xml);
        }

        [Fact]
        public void UnknownType_FallsBackToString()
        {
            var entry = new InfoPlistEntry
            {
                Key = "SomeKey",
                Type = "unknown_type",
                Value = "hello"
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<string>hello</string>", xml);
        }

        [Theory]
        [InlineData("String")]
        [InlineData("STRING")]
        [InlineData("sTrInG")]
        public void TypeIsCaseInsensitive(string type)
        {
            var entry = new InfoPlistEntry { Key = "K", Type = type, Value = "V" };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<string>V</string>", xml);
        }
    }

    public class BooleanTypeTests
    {
        [Theory]
        [InlineData("true", "<true/>")]
        [InlineData("false", "<false/>")]
        public void GeneratesCorrectBoolElement(string value, string expected)
        {
            var entry = new InfoPlistEntry
            {
                Key = "UIRequiresPersistentWiFi",
                Type = "boolean",
                Value = value
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<key>UIRequiresPersistentWiFi</key>", xml);
            Assert.Contains(expected, xml);
        }

        [Theory]
        [InlineData("True", "<true/>")]
        [InlineData("TRUE", "<true/>")]
        [InlineData("False", "<false/>")]
        [InlineData("FALSE", "<false/>")]
        public void ValueIsCaseInsensitive(string value, string expected)
        {
            var entry = new InfoPlistEntry { Key = "K", Type = "boolean", Value = value };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains(expected, xml);
        }

        [Theory]
        [InlineData("boolean")]
        [InlineData("bool")]
        [InlineData("Boolean")]
        [InlineData("Bool")]
        public void AcceptsBoolAndBooleanType(string type)
        {
            var entry = new InfoPlistEntry { Key = "K", Type = type, Value = "true" };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<true/>", xml);
        }

        [Fact]
        public void NonTrueValue_TreatedAsFalse()
        {
            var entry = new InfoPlistEntry { Key = "K", Type = "boolean", Value = "yes" };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<false/>", xml);
        }
    }

    public class ArrayTypeTests
    {
        [Fact]
        public void GeneratesArrayWithSemicolonDelimitedItems()
        {
            var entry = new InfoPlistEntry
            {
                Key = "BGTaskSchedulerPermittedIdentifiers",
                Type = "array",
                Value = "com.shiny.job;com.shiny.jobpower;com.shiny.jobnet;com.shiny.jobpowernet"
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<key>BGTaskSchedulerPermittedIdentifiers</key>", xml);
            Assert.Contains("<array>", xml);
            Assert.Contains("<string>com.shiny.job</string>", xml);
            Assert.Contains("<string>com.shiny.jobpower</string>", xml);
            Assert.Contains("<string>com.shiny.jobnet</string>", xml);
            Assert.Contains("<string>com.shiny.jobpowernet</string>", xml);
            Assert.Contains("</array>", xml);
        }

        [Fact]
        public void SingleItem_StillWrappedInArray()
        {
            var entry = new InfoPlistEntry
            {
                Key = "K",
                Type = "array",
                Value = "single.item"
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<array>", xml);
            Assert.Contains("<string>single.item</string>", xml);
            Assert.Contains("</array>", xml);
        }

        [Fact]
        public void EmptyValue_GeneratesEmptyArray()
        {
            var entry = new InfoPlistEntry { Key = "K", Type = "array", Value = "" };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<array>", xml);
            Assert.Contains("</array>", xml);
            Assert.DoesNotContain("<string>", xml);
        }

        [Fact]
        public void ConsecutiveSemicolons_EmptyEntriesSkipped()
        {
            var entry = new InfoPlistEntry { Key = "K", Type = "array", Value = "a;;;b" };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            var stringCount = xml.Split(new[] { "<string>" }, StringSplitOptions.None).Length - 1;
            Assert.Equal(2, stringCount);
        }

        [Fact]
        public void WhitespaceAroundItems_IsTrimmed()
        {
            var entry = new InfoPlistEntry
            {
                Key = "K",
                Type = "array",
                Value = " item1 ; item2 ; item3 "
            };
            var xml = InfoPlistGenerator.GenerateEntry(entry);

            Assert.Contains("<string>item1</string>", xml);
            Assert.Contains("<string>item2</string>", xml);
            Assert.Contains("<string>item3</string>", xml);
        }

        [Fact]
        public void ArrayItems_IndentedInsideArray()
        {
            var entry = new InfoPlistEntry { Key = "K", Type = "array", Value = "a;b" };
            var xml = InfoPlistGenerator.GenerateEntry(entry);
            var lines = xml.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var stringLines = lines.Where(l => l.Contains("<string>")).ToArray();

            foreach (var line in stringLines)
            {
                Assert.StartsWith("    <string>", line);
            }
        }
    }

    public class GeneratePlistXmlTests
    {
        [Fact]
        public void MatchesSpecExample()
        {
            var entries = new[]
            {
                new InfoPlistEntry
                {
                    Key = "NSAppleMusicUsageDescription",
                    Type = "string",
                    Value = "TuneGames needs access to your music library to play music trivia games"
                },
                new InfoPlistEntry
                {
                    Key = "UIRequiresPersistentWiFi",
                    Type = "boolean",
                    Value = "false"
                },
                new InfoPlistEntry
                {
                    Key = "BGTaskSchedulerPermittedIdentifiers",
                    Type = "array",
                    Value = "com.shiny.job;com.shiny.jobpower;com.shiny.jobnet;com.shiny.jobpowernet"
                },
            };

            var xml = InfoPlistGenerator.GeneratePlistXml(entries);

            Assert.Contains("<plist>", xml);
            Assert.Contains("<dict>", xml);
            Assert.Contains("</dict>", xml);
            Assert.Contains("</plist>", xml);
            Assert.Contains("<key>NSAppleMusicUsageDescription</key>", xml);
            Assert.Contains("<string>TuneGames needs access to your music library to play music trivia games</string>", xml);
            Assert.Contains("<false/>", xml);
            Assert.Contains("<array>", xml);
            Assert.Contains("<string>com.shiny.job</string>", xml);
        }

        [Fact]
        public void EmptyCollection_ValidPlistShell()
        {
            var xml = InfoPlistGenerator.GeneratePlistXml([]);

            Assert.Contains("<plist>", xml);
            Assert.Contains("<dict>", xml);
            Assert.Contains("</dict>", xml);
            Assert.Contains("</plist>", xml);
            Assert.DoesNotContain("<key>", xml);
        }

        [Fact]
        public void SingleEntry_ValidOutput()
        {
            var entries = new[]
            {
                new InfoPlistEntry { Key = "NSCameraUsageDescription", Type = "string", Value = "Camera access" },
            };

            var xml = InfoPlistGenerator.GeneratePlistXml(entries);

            Assert.Contains("<key>NSCameraUsageDescription</key>", xml);
            Assert.Contains("<string>Camera access</string>", xml);
        }

        [Fact]
        public void Entries_IndentedInsideDict()
        {
            var entries = new[]
            {
                new InfoPlistEntry { Key = "K", Type = "string", Value = "V" },
            };

            var xml = InfoPlistGenerator.GeneratePlistXml(entries);
            var lines = xml.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var keyLine = lines.First(l => l.Contains("<key>"));

            Assert.StartsWith("        <key>", keyLine);
        }

        [Fact]
        public void StructureOrder_PlistDictContentEndDictEndPlist()
        {
            var xml = InfoPlistGenerator.GeneratePlistXml([]);

            var plistOpen = xml.IndexOf("<plist>");
            var dictOpen = xml.IndexOf("<dict>");
            var dictClose = xml.IndexOf("</dict>");
            var plistClose = xml.IndexOf("</plist>");

            Assert.True(plistOpen < dictOpen);
            Assert.True(dictOpen < dictClose);
            Assert.True(dictClose < plistClose);
        }
    }
}
