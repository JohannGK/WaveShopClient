using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WaveShopClient.Pages;

public class ProductModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public ProductModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
