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
    [BindProperty]
    public List<Product> ProductsSelected { get; set; } = new List<Product>();
    [BindProperty]
    public User Client { get; set; } = new User();
    [BindProperty]
    public string ShopType { get; set; } = "Unitaria";
    [BindProperty]
    public string ViewType { get; set; } = "Success";

    public ShopNoteModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        ViewType = "Success";
    }

    public async Task OnPostShow(string tipoCompra, string idUser, string idProduct = null, string productQuantity = null)
    {
        
    }

    public async Task OnPost(string tipoCompra, string idUser, string idProduct = null, string productQuantity = null)
    {
        ViewType = "Process";
        if (tipoCompra == "Carrito")
            await ShopShoppingCart(idUser);
        else
            await ShopProduct(idUser, idProduct, productQuantity);
        RedirectToPage("./Shoppings");

    }

    private async Task ShopShoppingCart(string idUser)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Orders/");
        HttpResponseMessage response = await client.PostAsync($"{idUser}", null);
        if (!response.IsSuccessStatusCode)
            throw new Exception("No fue posible comprar los productos del carrito");
    }

    private async Task ShopProduct(string idUser, string idProduct, string quantity)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Orders/buy/");
        var product = await GetProduct(idProduct);
        product.StockQuantity = Convert.ToInt32(quantity);
        HttpResponseMessage response = await client.PostAsync(
            $"{idUser}",
            new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json")
        );
        if (!response.IsSuccessStatusCode)
            throw new Exception("No fue posible comprar el producto");
    }

    private async Task<Product> GetProduct(string productId)
    {
        Product? product = new Product();
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{productId}");
        if (response.IsSuccessStatusCode)
            product = await response.Content.ReadAsAsync<Product>();
        return product;
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