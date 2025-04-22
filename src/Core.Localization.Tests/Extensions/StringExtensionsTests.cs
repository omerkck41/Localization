using Core.Localization.Abstractions;
using Core.Localization.Extensions;
using FluentAssertions;
using Moq;
using System.Globalization;
using Xunit;

namespace Core.Localization.Tests.Extensions;

public class StringExtensionsTests
{
    private readonly Mock<ILocalizationService> _mockLocalizationService;

    public StringExtensionsTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService>();
    }

    [Fact]
    public void Localize_ReturnsCorrectValue()
    {
        // Arrange
        var key = "TestKey";
        var expectedValue = "Localized Value";

        _mockLocalizationService.Setup(x => x.GetString(key, (CultureInfo?)null))
            .Returns(expectedValue);

        // Act
        var result = key.Localize(_mockLocalizationService.Object);

        // Assert
        result.Should().Be(expectedValue);
        _mockLocalizationService.Verify(x => x.GetString(key, (CultureInfo?)null), Times.Once);
    }

    [Fact]
    public void Localize_WithCulture_ReturnsCorrectValue()
    {
        // Arrange
        var key = "TestKey";
        var culture = new CultureInfo("tr-TR");
        var expectedValue = "Yerelleştirilmiş Değer";

        _mockLocalizationService.Setup(x => x.GetString(key, culture))
            .Returns(expectedValue);

        // Act
        var result = key.Localize(_mockLocalizationService.Object, culture);

        // Assert
        result.Should().Be(expectedValue);
        _mockLocalizationService.Verify(x => x.GetString(key, culture), Times.Once);
    }

    [Fact]
    public void Localize_WithArgs_ReturnsCorrectValue()
    {
        // Arrange
        var key = "Welcome";
        var args = new object[] { "John" };
        var expectedValue = "Welcome, John!";

        _mockLocalizationService.Setup(x => x.GetString(key, (object[])args))
            .Returns(expectedValue);

        // Act
        var result = key.Localize(_mockLocalizationService.Object, args);

        // Assert
        result.Should().Be(expectedValue);
        _mockLocalizationService.Verify(x => x.GetString(key, args), Times.Once);
    }

    [Fact]
    public void Localize_WithCultureAndArgs_ReturnsCorrectValue()
    {
        // Arrange
        var key = "Welcome";
        var culture = new CultureInfo("tr-TR");
        var args = new object[] { "Ahmet" };
        var expectedValue = "Hoş geldin, Ahmet!";

        _mockLocalizationService.Setup(x => x.GetString(key, culture, (object[])args))
            .Returns(expectedValue);

        // Act
        var result = key.Localize(_mockLocalizationService.Object, culture, args);

        // Assert
        result.Should().Be(expectedValue);
        _mockLocalizationService.Verify(x => x.GetString(key, culture, args), Times.Once);
    }

    [Fact]
    public void TryLocalize_ReturnsTrue_WhenValueExists()
    {
        // Arrange
        var key = "TestKey";
        var expectedValue = "Localized Value";

        _mockLocalizationService.Setup(x => x.TryGetString(key, out expectedValue, (CultureInfo?)null))
            .Returns(true);

        // Act
        var result = key.TryLocalize(_mockLocalizationService.Object, out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(expectedValue);
    }

    [Fact]
    public void TryLocalize_ReturnsFalse_WhenValueDoesNotExist()
    {
        // Arrange
        var key = "MissingKey";
        string? nullValue = null;

        _mockLocalizationService.Setup(x => x.TryGetString(key, out nullValue, null))
            .Returns(false);

        // Act
        var result = key.TryLocalize(_mockLocalizationService.Object, out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryLocalize_WithCulture_ReturnsCorrectValue()
    {
        // Arrange
        var key = "TestKey";
        var culture = new CultureInfo("tr-TR");
        var expectedValue = "Yerelleştirilmiş Değer";

        _mockLocalizationService.Setup(x => x.TryGetString(key, out expectedValue, culture))
            .Returns(true);

        // Act
        var result = key.TryLocalize(_mockLocalizationService.Object, culture, out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(expectedValue);
        _mockLocalizationService.Verify(x => x.TryGetString(key, out expectedValue, culture), Times.Once);
    }
}
