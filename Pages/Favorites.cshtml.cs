using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class FavoritesModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public List<Product> Products { get; set; } = new List<Product>();

    public FavoritesModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                var ID = HttpContext.Session.GetString("id");
                var client = GetPreparedClient("https://localhost:7278/api/Favorites/");
                HttpResponseMessage response = await client.GetAsync($"{ID}");
                if (response.IsSuccessStatusCode)
                {
                    Products = await response.Content.ReadAsAsync<List<Product>>();
                    return Page();
                }
                throw new Exception();
            }
            catch (Exception)
            {
                return RedirectToPage("./Error", string.Empty, new { code = 500 });
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public async Task<ActionResult> OnPostRemove(string ID)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                var userId = HttpContext.Session.GetString("id");
                var client = GetPreparedClient("https://localhost:7278/api/Favorites/");
                HttpResponseMessage response = await client.DeleteAsync($"{userId}/{ID}");
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Producto removido", urlRedirection = "/Favorites", urlTittle = "Ver favoritos" });
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("./Error", string.Empty, new { code = 500 });
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public HttpClient? GetPreparedClient(string uri)
    {
        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
