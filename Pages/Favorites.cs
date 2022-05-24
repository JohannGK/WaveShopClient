using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WaveShopClient.Pages;

public class FavoritesModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public FavoritesModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
