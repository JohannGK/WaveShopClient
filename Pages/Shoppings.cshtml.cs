using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ShoppingsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public List<Product> ProductsSelected { get; set; } = new List<Product>();
    [BindProperty]
    public string ViewToShow { get; set; } = "Request";

    public ShoppingsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                var client = GetPreparedClient("https://localhost:7278/api/Orders/");
                var id = HttpContext.Session.GetString("id");
                HttpResponseMessage response = await client.GetAsync($"{id}");
                if (response.IsSuccessStatusCode)
                {
                    ProductsSelected = await response.Content.ReadAsAsync<List<Product>>();
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
