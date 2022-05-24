using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WaveShopClient.Pages;

public class ShoppingsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public ShoppingsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async void OnGet()
    {

    }
}
