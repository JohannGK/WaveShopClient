using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ShopNoteModel : PageModel
{
    [BindProperty]
    public string IsCorrect { get; set; } = "Yes";
    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;
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

    public async Task<IActionResult> OnPostShopProduct(string id, string quantity)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
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
                    return RedirectToPage("./Success", string.Empty, new { header = "Producto comprado", urlRedirection = "/Shoppings", urlTittle = "Ver compras" });
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                ErrorMessage = json["value"]["error"].ToString();
                throw new Exception(ErrorMessage);
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Error", "CustomError", new { code = 113, message = ex.Message });
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public async Task<IActionResult> OnPostShopCart()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                var idUser = HttpContext.Session.GetString("id");
                var client = GetPreparedClient("https://localhost:7278/api/Orders/");
                HttpResponseMessage response = await client.PostAsync($"{idUser}", null);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Producto comprado", urlRedirection = "/Shoppings", urlTittle = "Ver compras" });
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                ErrorMessage = json["value"]["error"].ToString();
                throw new Exception(ErrorMessage);
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Error", "CustomError", new { code = 113, message = ex.Message });
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public async Task<ActionResult> OnGetShoppingCartShow()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                await GetShoppingCart(HttpContext.Session.GetString("id"));
                await GetUser(HttpContext.Session.GetString("id"));
                ShopType = "Carrito";
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

    public async Task<ActionResult> OnGetUnitProductShow(string id, string quantity)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                await GetUser(HttpContext.Session.GetString("id"));
                ProductSelected = await GetProduct(id);
                ProductSelected.StockQuantity = Convert.ToInt32(quantity);
                Total = ProductSelected.UnitPrice * ProductSelected.StockQuantity;
                ShopType = "Unitaria";
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