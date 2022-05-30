using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WaveShopClient.Pages;

public class ShoppingCartModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public ShoppingCartModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async void OnGet()
    {

    }

    
}
