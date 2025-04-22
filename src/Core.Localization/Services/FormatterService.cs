using System.Globalization;
using Core.Localization.Abstractions;

namespace Core.Localization.Services;

/// <summary>
/// Implementation of formatting services for various data types
/// </summary>
public class FormatterService : IFormatterService
{
    public string FormatDate(DateTime date, string? format = null, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return date.ToString(format ?? culture.DateTimeFormat.ShortDatePattern, culture);
    }

    public string FormatNumber(decimal number, string? format = null, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return number.ToString(format ?? "N2", culture);
    }

    public string FormatCurrency(decimal amount, string? currencyCode = null, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        
        if (currencyCode != null)
        {
            // Create a custom NumberFormatInfo with the specified currency
            var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
            numberFormat.CurrencySymbol = currencyCode;
            return amount.ToString("C", numberFormat);
        }
        
        return amount.ToString("C", culture);
    }

    public string FormatPercentage(decimal value, int decimals = 2, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        var format = $"P{decimals}";
        return value.ToString(format, culture);
    }

    public DateTime? ParseDate(string dateString, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        
        if (DateTime.TryParse(dateString, culture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        // Try parsing with various standard formats
        string[] formats = { 
            culture.DateTimeFormat.ShortDatePattern,
            culture.DateTimeFormat.LongDatePattern,
            culture.DateTimeFormat.ShortTimePattern,
            culture.DateTimeFormat.LongTimePattern,
            culture.DateTimeFormat.FullDateTimePattern,
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        if (DateTime.TryParseExact(dateString, formats, culture, DateTimeStyles.None, out result))
        {
            return result;
        }

        return null;
    }

    public decimal? ParseNumber(string numberString, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        
        if (decimal.TryParse(numberString, NumberStyles.Number, culture, out var result))
        {
            return result;
        }

        return null;
    }

    public decimal? ParseCurrency(string currencyString, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        
        // Remove any currency symbols and spaces
        var cleanedString = currencyString.Trim();
        foreach (var symbol in new[] { culture.NumberFormat.CurrencySymbol, "$", "€", "£", "¥", "₺" })
        {
            cleanedString = cleanedString.Replace(symbol, "");
        }
        
        cleanedString = cleanedString.Trim();
        
        if (decimal.TryParse(cleanedString, NumberStyles.Currency, culture, out var result))
        {
            return result;
        }

        return null;
    }
}
