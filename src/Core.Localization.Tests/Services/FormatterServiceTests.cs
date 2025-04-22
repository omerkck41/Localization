using System.Globalization;
using Core.Localization.Services;
using FluentAssertions;
using Xunit;

namespace Core.Localization.Tests.Services;

public class FormatterServiceTests
{
    private readonly FormatterService _formatterService;

    public FormatterServiceTests()
    {
        _formatterService = new FormatterService();
    }

    [Fact]
    public void FormatDate_FormatsCorrectly_WithDefaultPattern()
    {
        // Arrange
        var date = new DateTime(2025, 1, 15);
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatDate(date, culture: culture);

        // Assert
        result.Should().Be("1/15/2025");
    }

    [Fact]
    public void FormatDate_FormatsCorrectly_WithCustomPattern()
    {
        // Arrange
        var date = new DateTime(2025, 1, 15);
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatDate(date, "yyyy-MM-dd", culture);

        // Assert
        result.Should().Be("2025-01-15");
    }

    [Fact]
    public void FormatNumber_FormatsCorrectly_WithDefaultPattern()
    {
        // Arrange
        var number = 1234.56m;
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatNumber(number, culture: culture);

        // Assert
        result.Should().Be("1,234.56");
    }

    [Fact]
    public void FormatNumber_FormatsCorrectly_WithCustomPattern()
    {
        // Arrange
        var number = 1234.56m;
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatNumber(number, "N3", culture);

        // Assert
        result.Should().Be("1,234.560");
    }

    [Fact]
    public void FormatCurrency_FormatsCorrectly_WithDefaultCurrency()
    {
        // Arrange
        var amount = 1234.56m;
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatCurrency(amount, culture: culture);

        // Assert
        result.Should().Be("$1,234.56");
    }

    [Fact]
    public void FormatCurrency_FormatsCorrectly_WithCustomCurrency()
    {
        // Arrange
        var amount = 1234.56m;
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatCurrency(amount, "€", culture);

        // Assert
        result.Should().Be("€1,234.56");
    }

    [Fact]
    public void FormatPercentage_FormatsCorrectly()
    {
        // Arrange
        var value = 0.1234m;
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.FormatPercentage(value, 2, culture);

        // Assert
        result.Should().Be("12.34%");
    }

    [Fact]
    public void ParseDate_ParsesCorrectly_WithStandardFormat()
    {
        // Arrange
        var dateString = "1/15/2025";
        var culture = new CultureInfo("en-US");
        var expected = new DateTime(2025, 1, 15);

        // Act
        var result = _formatterService.ParseDate(dateString, culture);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParseDate_ParsesCorrectly_WithISOFormat()
    {
        // Arrange
        var dateString = "2025-01-15";
        var culture = new CultureInfo("en-US");
        var expected = new DateTime(2025, 1, 15);

        // Act
        var result = _formatterService.ParseDate(dateString, culture);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParseDate_ReturnsNull_ForInvalidDate()
    {
        // Arrange
        var dateString = "invalid date";
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.ParseDate(dateString, culture);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseNumber_ParsesCorrectly()
    {
        // Arrange
        var numberString = "1,234.56";
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.ParseNumber(numberString, culture);

        // Assert
        result.Should().Be(1234.56m);
    }

    [Fact]
    public void ParseNumber_ReturnsNull_ForInvalidNumber()
    {
        // Arrange
        var numberString = "invalid number";
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.ParseNumber(numberString, culture);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseCurrency_ParsesCorrectly_WithCurrencySymbol()
    {
        // Arrange
        var currencyString = "$1,234.56";
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.ParseCurrency(currencyString, culture);

        // Assert
        result.Should().Be(1234.56m);
    }

    [Fact]
    public void ParseCurrency_ParsesCorrectly_WithoutCurrencySymbol()
    {
        // Arrange
        var currencyString = "1,234.56";
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.ParseCurrency(currencyString, culture);

        // Assert
        result.Should().Be(1234.56m);
    }

    [Fact]
    public void ParseCurrency_ReturnsNull_ForInvalidCurrency()
    {
        // Arrange
        var currencyString = "invalid currency";
        var culture = new CultureInfo("en-US");

        // Act
        var result = _formatterService.ParseCurrency(currencyString, culture);

        // Assert
        result.Should().BeNull();
    }
}
