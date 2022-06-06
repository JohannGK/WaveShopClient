using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ProductsSoldDetailsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public List<Product> Products { get; set; } = new List<Product>();
    [BindProperty]
    public Product ProductSelected { get; set; }
    [BindProperty]
    public string View { get; set; }

    public ProductsSoldDetailsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGet(string id)
    {
        var ID = HttpContext.Session.GetString("id");
        await GetProduct(id);
        var client = GetPreparedClient("https://localhost:7278/api/Orders/sold/details/");
        HttpResponseMessage response = await client.GetAsync($"{id}");
        if (response.IsSuccessStatusCode)
            Products = await response.Content.ReadAsAsync<List<Product>>();
        View = "Request";
    }

    public async Task OnGetAddSuccess()
    {
        View = "AddSuccess";
    }

    private async Task<Product> GetProduct(string productId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{productId}");
        if (response.IsSuccessStatusCode)
            ProductSelected = await response.Content.ReadAsAsync<Product>();
        return ProductSelected;
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
