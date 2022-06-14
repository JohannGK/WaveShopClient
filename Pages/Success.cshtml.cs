using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class SuccessModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public string Header { get; set; } = "Todo bien!";
    [BindProperty]
    public string UrlToRedirect { get; set; } = "/Index";
    [BindProperty]
    public string UrlTitle { get; set; } = "Volver a inicio";

    public SuccessModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string header, string urlRedirection, string urlTittle)
    {
        Header = header;
        UrlToRedirect = urlRedirection;
        UrlTitle = urlTittle;
    }
}