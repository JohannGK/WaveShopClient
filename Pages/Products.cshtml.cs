using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ProductsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public List<Product> ProductsList { get; set; } = new List<Product>();
    [BindProperty]
    public string SearchValue { get; set; } = string.Empty;
    [BindProperty]
    public List<Category> Categories { get; set; } = new List<Category>();

    public ProductsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public HttpClient? GetPreparedClient(string url)
    {
        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<ActionResult> OnGet(string searchTxt, int category)
    {
        SearchValue = searchTxt;
        try
        {
            if (string.IsNullOrEmpty(searchTxt))
            {
                ProductsList = new List<Product>();
                return Page();
            }
            else
            {
                await GetCategories();
                await GetProducts(category, searchTxt);
                return Page();
            }
        }
        catch (Exception ex)
        {
            return RedirectToPage("./Error", string.Empty, new { code = 500 });
        }
    }

    public async Task<List<Product>> GetProducts(int category, string searchTxt)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{category}/{searchTxt}");
        if (response.IsSuccessStatusCode)
            return ProductsList = await response.Content.ReadAsAsync<List<Product>>();
        else
            throw new Exception("500");
    }

    private async Task<List<Category>> GetCategories()
    {
        var client = GetPreparedClient("https://localhost:7278/api/Categories/");
        HttpResponseMessage response = await client.GetAsync($"");
        if (response.IsSuccessStatusCode)
            return Categories = await response.Content.ReadAsAsync<List<Category>>();
        throw new Exception();
    }

}
