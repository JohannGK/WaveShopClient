using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ProductModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public Product ProductSelected { get; set; } = new Product();
    [BindProperty]
    public User UserVentor { get; set; } = new User();
    [BindProperty]
    public string Quantity { get; set; } = "1";

    public ProductModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
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

    public async Task OnGet(string ID)
    {
        await GetProduct(ID);
        await GetUserVentor(ProductSelected.IdVendor.ToString());
    }

    private async Task<Product> GetProduct(string productId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{productId}");
        if (response.IsSuccessStatusCode)
            ProductSelected = await response.Content.ReadAsAsync<Product>();
        return ProductSelected;
    }

    private async Task<User> GetUserVentor(string userId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Users/");
        HttpResponseMessage response = await client.GetAsync($"{userId}");
        if (response.IsSuccessStatusCode)
            UserVentor = await response.Content.ReadAsAsync<User>();
        return UserVentor;
    }

    public async Task<IActionResult> OnPostBuyProduct(string idProduct, string productQuantity)
    {
        await GetProduct(idProduct);
        if (ProductSelected.StockQuantity >= Convert.ToInt32(productQuantity))
            return RedirectToPage("./ShopNote", "UnitProductShow", new { id = idProduct, quantity = productQuantity });
        else
            await GetUserVentor(ProductSelected.IdVendor.ToString());
        return Page();
    }

    public async Task<IActionResult> OnPostAddShoppingCart(string idProduct, string productQuantity)
    {
        await GetProduct(idProduct);
        if (ProductSelected.StockQuantity >= Convert.ToInt32(productQuantity))
        {
            ProductSelected.StockQuantity = Convert.ToInt32(productQuantity);
            var idUser = HttpContext.Session.GetString("id");
            var client = GetPreparedClient("https://localhost:7278/api/Products/");
            HttpResponseMessage response = await client.PostAsync
            (
                $"{idUser}",
                new StringContent(JsonConvert.SerializeObject(ProductSelected), Encoding.UTF8, "application/json")
            );
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./ShoppingCart", "AddSuccess");
            }
            else
            {
                await GetUserVentor(ProductSelected.IdVendor.ToString());
                return Page();
            }
        }
        else
        {
            await GetUserVentor(ProductSelected.IdVendor.ToString());
            return Page();
        }
    }
}
