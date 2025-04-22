using System.Globalization;
using Core.Localization.Providers;
using FluentAssertions;
using Xunit;

namespace Core.Localization.Tests.Providers;

public class InMemoryResourceProviderTests
{
    private readonly InMemoryResourceProvider _provider;

    public InMemoryResourceProviderTests()
    {
        var resources = new Dictionary<string, IDictionary<string, string>>
        {
            ["en-US"] = new Dictionary<string, string>
            {
                ["Hello"] = "Hello",
                ["Welcome"] = "Welcome to {0}",
                ["Goodbye"] = "Goodbye"
            },
            ["tr-TR"] = new Dictionary<string, string>
            {
                ["Hello"] = "Merhaba",
                ["Welcome"] = "{0} hoş geldiniz",
                ["Goodbye"] = "Güle güle"
            }
        };

        _provider = new InMemoryResourceProvider(resources);
    }

    [Fact]
    public void GetString_ReturnsCorrectValue_ForExistingKey()
    {
        // Arrange
        var culture = new CultureInfo("tr-TR");

        // Act
        var result = _provider.GetString("Hello", culture);

        // Assert
        result.Should().Be("Merhaba");
    }

    [Fact]
    public void GetString_ReturnsNull_ForNonExistingKey()
    {
        // Arrange
        var culture = new CultureInfo("tr-TR");

        // Act
        var result = _provider.GetString("NonExisting", culture);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetString_FallsBackToParentCulture_WhenKeyNotFound()
    {
        // Arrange
        var culture = new CultureInfo("tr");
        _provider.AddOrUpdateResource("tr", "Test", "Test TR");

        // Act
        var result = _provider.GetString("Test", new CultureInfo("tr-TR"));

        // Assert
        result.Should().Be("Test TR");
    }

    [Fact]
    public void GetAllKeys_ReturnsAllKeys_ForCulture()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        // Act
        var keys = _provider.GetAllKeys(culture).ToList();

        // Assert
        keys.Should().Contain(new[] { "Hello", "Welcome", "Goodbye" });
        keys.Should().HaveCount(3);
    }

    [Fact]
    public void AddOrUpdateResource_AddsNewResource()
    {
        // Act
        _provider.AddOrUpdateResource("fr-FR", "Hello", "Bonjour");
        var result = _provider.GetString("Hello", new CultureInfo("fr-FR"));

        // Assert
        result.Should().Be("Bonjour");
    }

    [Fact]
    public void AddOrUpdateResource_UpdatesExistingResource()
    {
        // Act
        _provider.AddOrUpdateResource("en-US", "Hello", "Hi");
        var result = _provider.GetString("Hello", new CultureInfo("en-US"));

        // Assert
        result.Should().Be("Hi");
    }

    [Fact]
    public void RemoveResource_RemovesExistingResource()
    {
        // Act
        var removed = _provider.RemoveResource("en-US", "Hello");
        var result = _provider.GetString("Hello", new CultureInfo("en-US"));

        // Assert
        removed.Should().BeTrue();
        result.Should().BeNull();
    }

    [Fact]
    public void RemoveResource_ReturnsFalse_ForNonExistingResource()
    {
        // Act
        var removed = _provider.RemoveResource("en-US", "NonExisting");

        // Assert
        removed.Should().BeFalse();
    }

    [Fact]
    public void Clear_RemovesAllResources()
    {
        // Act
        _provider.Clear();
        var result = _provider.GetString("Hello", new CultureInfo("en-US"));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void HasKey_ReturnsTrue_ForExistingKey()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        // Act
        var result = _provider.HasKey("Hello", culture);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasKey_ReturnsFalse_ForNonExistingKey()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        // Act
        var result = _provider.HasKey("NonExisting", culture);

        // Assert
        result.Should().BeFalse();
    }
}
