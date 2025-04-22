using System.Globalization;
using Core.Localization.Abstractions;
using Core.Localization.Configuration;
using Core.Localization.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Core.Localization.Tests.Services;

public class LocalizationServiceTests
{
    private readonly Mock<IResourceProvider> _mockProvider;
    private readonly Mock<IOptions<LocalizationOptions>> _mockOptions;
    private readonly Mock<ILogger<LocalizationService>> _mockLogger;
    private readonly LocalizationService _service;
    private readonly LocalizationOptions _options;

    public LocalizationServiceTests()
    {
        _mockProvider = new Mock<IResourceProvider>();
        _mockOptions = new Mock<IOptions<LocalizationOptions>>();
        _mockLogger = new Mock<ILogger<LocalizationService>>();
        
        _options = new LocalizationOptions
        {
            DefaultCulture = new CultureInfo("en-US"),
            FallbackCulture = new CultureInfo("en-US"),
            SupportedCultures = new List<CultureInfo> 
            { 
                new CultureInfo("en-US"), 
                new CultureInfo("tr-TR") 
            }
        };
        
        _mockOptions.Setup(x => x.Value).Returns(_options);
        _mockProvider.Setup(x => x.Priority).Returns(100);
        
        _service = new LocalizationService(
            new[] { _mockProvider.Object },
            _mockOptions.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public void GetString_ReturnsLocalizedString_WhenKeyExists()
    {
        // Arrange
        var key = "Hello";
        var culture = new CultureInfo("tr-TR");
        var expectedValue = "Merhaba";

        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns(expectedValue);

        // Act
        var result = _service.GetString(key, culture);

        // Assert
        result.Should().Be(expectedValue);
        _mockProvider.Verify(x => x.GetString(key, culture), Times.Once);
    }

    [Fact]
    public void GetString_ReturnsFallbackValue_WhenKeyNotFound()
    {
        // Arrange
        var key = "Hello";
        var requestedCulture = new CultureInfo("fr-FR");
        var fallbackCulture = _options.FallbackCulture;
        var fallbackValue = "Hello";

        _mockProvider.Setup(x => x.GetString(key, requestedCulture))
            .Returns((string?)null);
        _mockProvider.Setup(x => x.GetString(key, fallbackCulture))
            .Returns(fallbackValue);

        // Act
        var result = _service.GetString(key, requestedCulture);

        // Assert
        result.Should().Be(fallbackValue);
        _mockProvider.Verify(x => x.GetString(key, requestedCulture), Times.Once);
        _mockProvider.Verify(x => x.GetString(key, fallbackCulture), Times.Once);
    }

    [Fact]
    public void GetString_ReturnsKey_WhenNotFoundAndNoFallback()
    {
        // Arrange
        var key = "Missing";
        var culture = new CultureInfo("en-US");

        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns((string?)null);

        // Act
        var result = _service.GetString(key, culture);

        // Assert
        result.Should().Be(key);
    }

    [Fact]
    public void GetString_WithFormatArgs_FormatsCorrectly()
    {
        // Arrange
        var key = "Welcome";
        var culture = new CultureInfo("en-US");
        var format = "Welcome to {0}";
        var args = new object[] { "the application" };

        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns(format);

        // Act
        var result = _service.GetString(key, culture, args);

        // Assert
        result.Should().Be("Welcome to the application");
    }

    [Fact]
    public void GetString_ThrowsException_WhenConfiguredAndKeyNotFound()
    {
        // Arrange
        var key = "Missing";
        var culture = new CultureInfo("en-US");
        _options.ThrowOnMissingResource = true;

        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns((string?)null);

        // Act & Assert
        Action act = () => _service.GetString(key, culture);
        act.Should().Throw<KeyNotFoundException>()
            .WithMessage($"Resource key '{key}' not found for culture '{culture.Name}'");
    }

    [Fact]
    public void TryGetString_ReturnsTrue_WhenKeyExists()
    {
        // Arrange
        var key = "Hello";
        var culture = new CultureInfo("en-US");
        var expectedValue = "Hello";

        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns(expectedValue);

        // Act
        var result = _service.TryGetString(key, out var value, culture);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(expectedValue);
    }

    [Fact]
    public void TryGetString_ReturnsFalse_WhenKeyNotFound()
    {
        // Arrange
        var key = "Missing";
        var culture = new CultureInfo("en-US");

        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns((string?)null);
        _mockProvider.Setup(x => x.GetString(key, _options.FallbackCulture))
            .Returns((string?)null);

        // Act
        var result = _service.TryGetString(key, out var value, culture);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void GetAllStrings_ReturnsAllTranslations()
    {
        // Arrange
        var key = "Hello";
        var usValue = "Hello";
        var trValue = "Merhaba";

        _mockProvider.Setup(x => x.GetString(key, new CultureInfo("en-US")))
            .Returns(usValue);
        _mockProvider.Setup(x => x.GetString(key, new CultureInfo("tr-TR")))
            .Returns(trValue);

        // Act
        var result = _service.GetAllStrings(key);

        // Assert
        result.Should().HaveCount(2);
        result[new CultureInfo("en-US")].Should().Be(usValue);
        result[new CultureInfo("tr-TR")].Should().Be(trValue);
    }

    [Fact]
    public void GetAllKeys_ReturnsKeysFromProvider()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        var expectedKeys = new[] { "Key1", "Key2", "Key3" };

        _mockProvider.Setup(x => x.GetAllKeys(culture))
            .Returns(expectedKeys);

        // Act
        var result = _service.GetAllKeys(culture).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedKeys);
    }

    [Fact]
    public void GetSupportedCultures_ReturnsCulturesFromOptions()
    {
        // Act
        var result = _service.GetSupportedCultures();

        // Assert
        result.Should().BeEquivalentTo(_options.SupportedCultures);
    }

    [Fact]
    public void GetString_UsesCaching_WhenEnabled()
    {
        // Arrange
        var key = "Hello";
        var culture = new CultureInfo("en-US");
        var expectedValue = "Hello";
        var cache = new MemoryCache(new MemoryCacheOptions());
        
        _mockProvider.Setup(x => x.GetString(key, culture))
            .Returns(expectedValue);

        var serviceWithCache = new LocalizationService(
            new[] { _mockProvider.Object },
            _mockOptions.Object,
            _mockLogger.Object,
            cache
        );

        // Act
        var result1 = serviceWithCache.GetString(key, culture);
        var result2 = serviceWithCache.GetString(key, culture);

        // Assert
        result1.Should().Be(expectedValue);
        result2.Should().Be(expectedValue);
        _mockProvider.Verify(x => x.GetString(key, culture), Times.Once);
    }
}
