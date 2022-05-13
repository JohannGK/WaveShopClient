using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WaveShopClient.Pages;

public class ProductsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public ProductsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
