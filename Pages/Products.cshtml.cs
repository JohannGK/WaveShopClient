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

    public ProductsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public HttpClient? GetPreparedClient()
    {
        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:7278/api/Products/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<ActionResult> OnGet(string searchTxt)
    {
        SearchValue = searchTxt;
        try
        {
            var client = GetPreparedClient();
            if (client != null)
            {
                HttpResponseMessage response = await client.GetAsync($"-1/{searchTxt}");
                if (response.IsSuccessStatusCode)
                    ProductsList = await response.Content.ReadAsAsync<List<Product>>();
                else
                    throw new Exception("500");
                return Page();
            }
            else
            {
                throw new Exception("500");
            }
        }
        catch (Exception ex)
        {
            return RedirectToPage("./Error", string.Empty, new { code = int.Parse(ex.Message) });
        }
    }


}
