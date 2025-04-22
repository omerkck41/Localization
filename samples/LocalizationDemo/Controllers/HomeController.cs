using System.Globalization;
using Core.Localization.Abstractions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace LocalizationDemo.Controllers;

public class HomeController : Controller
{
    private readonly ILocalizationService _localization;
    private readonly IFormatterService _formatter;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILocalizationService localization, 
        IFormatterService formatter,
        ILogger<HomeController> logger)
    {
        _localization = localization;
        _formatter = formatter;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        
        ViewBag.CurrentCulture = currentCulture.Name;
        ViewBag.Greeting = _localization.GetString("Hello", currentCulture);
        ViewBag.Welcome = _localization.GetString("Welcome", currentCulture, "LocalizationDemo");
        ViewBag.CurrentDate = _formatter.FormatDate(DateTime.Now, culture: currentCulture);
        ViewBag.CurrentTime = _formatter.FormatDate(DateTime.Now, "HH:mm:ss", currentCulture);
        ViewBag.Currency = _formatter.FormatCurrency(1234.56m, culture: currentCulture);
        ViewBag.Number = _formatter.FormatNumber(1234567.89m, culture: currentCulture);
        ViewBag.Percentage = _formatter.FormatPercentage(0.1234m, culture: currentCulture);
        
        // Get all supported cultures for the language selector
        ViewBag.SupportedCultures = _localization.GetSupportedCultures();
        
        return View();
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

    public IActionResult Demo()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        
        // Add this line to set ViewBag.SupportedCultures
        ViewBag.SupportedCultures = _localization.GetSupportedCultures();
        
        // Demonstrate various localization features
        var demoData = new
        {
            Greeting = _localization.GetString("Hello", currentCulture),
            Welcome = _localization.GetString("Welcome", currentCulture, "Demo User"),
            Validation = new
            {
                Required = _localization.GetString("RequiredField", currentCulture, "Username"),
                MinLength = _localization.GetString("MinLength", currentCulture, "Password", 8),
                MaxLength = _localization.GetString("MaxLength", currentCulture, "Username", 20),
                EmailFormat = _localization.GetString("EmailFormat", currentCulture)
            },
            DateTime = new
            {
                Today = _localization.GetString("Today", currentCulture),
                Tomorrow = _localization.GetString("Tomorrow", currentCulture),
                Yesterday = _localization.GetString("Yesterday", currentCulture),
                ShortDate = _formatter.FormatDate(DateTime.Now, culture: currentCulture),
                LongDate = _formatter.FormatDate(DateTime.Now, "dddd, MMMM d, yyyy", currentCulture),
                Time = _formatter.FormatDate(DateTime.Now, "HH:mm:ss", currentCulture)
            },
            Numbers = new
            {
                Currency = _formatter.FormatCurrency(9876.54m, culture: currentCulture),
                Number = _formatter.FormatNumber(123456789.123m, culture: currentCulture),
                Percentage = _formatter.FormatPercentage(0.8765m, culture: currentCulture),
                LargeCurrency = _formatter.FormatCurrency(1234567.89m, culture: currentCulture)
            },
            Actions = new
            {
                Submit = _localization.GetString("Submit", currentCulture),
                Cancel = _localization.GetString("Cancel", currentCulture),
                Save = _localization.GetString("Save", currentCulture),
                Delete = _localization.GetString("Delete", currentCulture)
            }
        };
        
        return View(demoData);
    }
}
