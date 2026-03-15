using Shiny.Permissions.MSBuild.Generators;
using Shiny.Permissions.MSBuild.Models;

namespace Shiny.Permissions.MSBuild.Tests;

public class InfoPlistGeneratorTests
{
    [Fact]
    public Task Empty()
    {
        return Verify(InfoPlistGenerator.GeneratePlistXml([]));
    }

    [Fact]
    public Task SingleString()
    {
        var entries = new[]
        {
            new InfoPlistEntry
            {
                Key = "NSCameraUsageDescription",
                Type = "string",
                Value = "Camera access"
            },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task SingleBoolean()
    {
        var entries = new[]
        {
            new InfoPlistEntry
            {
                Key = "UIRequiresPersistentWiFi",
                Type = "boolean",
                Value = "true"
            },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task BooleanFalse()
    {
        var entries = new[]
        {
            new InfoPlistEntry
            {
                Key = "UIRequiresPersistentWiFi",
                Type = "boolean",
                Value = "false"
            },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task BoolShortType()
    {
        var entries = new[]
        {
            new InfoPlistEntry { Key = "K", Type = "bool", Value = "true" },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task SingleArray()
    {
        var entries = new[]
        {
            new InfoPlistEntry
            {
                Key = "BGTaskSchedulerPermittedIdentifiers",
                Type = "array",
                Value = "com.shiny.job;com.shiny.jobpower;com.shiny.jobnet"
            },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task ArraySingleItem()
    {
        var entries = new[]
        {
            new InfoPlistEntry { Key = "K", Type = "array", Value = "single.item" },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task ArrayTrimsWhitespace()
    {
        var entries = new[]
        {
            new InfoPlistEntry { Key = "K", Type = "array", Value = " item1 ; item2 ; item3 " },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task ArraySkipsEmptyEntries()
    {
        var entries = new[]
        {
            new InfoPlistEntry { Key = "K", Type = "array", Value = "a;;;b" },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task UnknownTypeFallsBackToString()
    {
        var entries = new[]
        {
            new InfoPlistEntry { Key = "SomeKey", Type = "unknown_type", Value = "hello" },
        };
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }

    [Fact]
    public Task MixedEntryTypes()
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
        return Verify(InfoPlistGenerator.GeneratePlistXml(entries));
    }
}
