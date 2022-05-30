using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages;

public class ProductFormModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public ProductFormModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
