using System.Globalization;

namespace Core.Localization.Abstractions;

/// <summary>
/// Provides formatting services for different data types
/// </summary>
public interface IFormatterService
{
    /// <summary>
    /// Formats a date according to culture-specific rules
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <param name="format">Optional format string</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Formatted date string</returns>
    string FormatDate(DateTime date, string? format = null, CultureInfo? culture = null);

    /// <summary>
    /// Formats a number according to culture-specific rules
    /// </summary>
    /// <param name="number">The number to format</param>
    /// <param name="format">Optional format string</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Formatted number string</returns>
    string FormatNumber(decimal number, string? format = null, CultureInfo? culture = null);

    /// <summary>
    /// Formats currency according to culture-specific rules
    /// </summary>
    /// <param name="amount">The amount to format</param>
    /// <param name="currencyCode">Optional currency code (e.g., USD, EUR)</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Formatted currency string</returns>
    string FormatCurrency(decimal amount, string? currencyCode = null, CultureInfo? culture = null);

    /// <summary>
    /// Formats a percentage according to culture-specific rules
    /// </summary>
    /// <param name="value">The value to format as percentage</param>
    /// <param name="decimals">Number of decimal places</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Formatted percentage string</returns>
    string FormatPercentage(decimal value, int decimals = 2, CultureInfo? culture = null);

    /// <summary>
    /// Parses a culture-specific date string
    /// </summary>
    /// <param name="dateString">The date string to parse</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Parsed DateTime or null if parsing fails</returns>
    DateTime? ParseDate(string dateString, CultureInfo? culture = null);

    /// <summary>
    /// Parses a culture-specific number string
    /// </summary>
    /// <param name="numberString">The number string to parse</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Parsed decimal or null if parsing fails</returns>
    decimal? ParseNumber(string numberString, CultureInfo? culture = null);

    /// <summary>
    /// Parses a culture-specific currency string
    /// </summary>
    /// <param name="currencyString">The currency string to parse</param>
    /// <param name="culture">Optional culture, defaults to current culture</param>
    /// <returns>Parsed decimal or null if parsing fails</returns>
    decimal? ParseCurrency(string currencyString, CultureInfo? culture = null);
}
