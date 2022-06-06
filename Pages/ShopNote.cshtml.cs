using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ShopNoteModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public List<Product> ProductsSelected { get; set; } = new List<Product>();
    public Product ProductSelected { get; set; } = new Product();
    public User Client { get; set; } = new User();
    public string ShopType { get; set; }
    public double Total { get; set; } = 0;

    public ShopNoteModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostShopProduct(string id, string quantity)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            var idUser = HttpContext.Session.GetString("id");
            var product = await GetProduct(id);
            product.StockQuantity = Convert.ToInt32(quantity);
            var client = GetPreparedClient("https://localhost:7278/api/Orders/buy/");
            HttpResponseMessage response = await client.PostAsync(
                $"{idUser}",
                new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json")
            );
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Shoppings", "ShopSuccess");
            else
                return Page();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostShopCart()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            var idUser = HttpContext.Session.GetString("id");
            var client = GetPreparedClient("https://localhost:7278/api/Orders/");
            HttpResponseMessage response = await client.PostAsync($"{idUser}", null);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Shoppings", "ShopSuccess");
            else
                return Page();
        }
        return Page();
    }

    public async Task OnGetShoppingCartShow()
    {
        await GetShoppingCart(HttpContext.Session.GetString("id"));
        await GetUser(HttpContext.Session.GetString("id"));
        ShopType = "Carrito";
    }

    public async Task OnGetUnitProductShow(string id, string quantity)
    {
        await GetUser(HttpContext.Session.GetString("id"));
        ProductSelected = await GetProduct(id);
        ProductSelected.StockQuantity = Convert.ToInt32(quantity);
        Total = ProductSelected.UnitPrice * ProductSelected.StockQuantity;
        ShopType = "Unitaria";
    }

    private async Task<Product> GetProduct(string productId)
    {
        Product? product = new Product();
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{productId}");
        if (response.IsSuccessStatusCode)
        {
            product = await response.Content.ReadAsAsync<Product>();
        }
        return product;
    }

    private async Task<List<Product>> GetShoppingCart(string userId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/ShoppingCart/");
        HttpResponseMessage response = await client.GetAsync($"{userId}");
        if (response.IsSuccessStatusCode)
        {
            ProductsSelected = await response.Content.ReadAsAsync<List<Product>>();
            ProductsSelected.ForEach(p => Total += p.UnitPrice * p.StockQuantity);
        }
        return ProductsSelected;
    }

    private async Task<User> GetUser(string userId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Users/");
        HttpResponseMessage response = await client.GetAsync($"{userId}");
        if (response.IsSuccessStatusCode)
            Client = await response.Content.ReadAsAsync<User>();
        return Client;
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