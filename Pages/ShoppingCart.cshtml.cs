using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ShoppingCartModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public List<Product> Products { get; set; } = new List<Product>();
    [BindProperty]
    public string View { get; set; } = "Request";

    public ShoppingCartModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGet()
    {
        var ID = HttpContext.Session.GetString("id");
        View = "Request";
        var client = GetPreparedClient("https://localhost:7278/api/ShoppingCart/");
        HttpResponseMessage response = await client.GetAsync($"{ID}");
        if (response.IsSuccessStatusCode)
            Products = await response.Content.ReadAsAsync<List<Product>>();
    }

    public async Task OnGetAddSuccess()
    {
        View = "AddSuccess";
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
