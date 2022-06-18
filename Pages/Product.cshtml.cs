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
    public List<Comment> Comments { get; set; } = new List<Comment>();
    [BindProperty]
    public User UserVentor { get; set; }
    [BindProperty]
    public string Quantity { get; set; } = "1";
    [BindProperty]
    public string IsCorrect { get; set; } = "Yes";
    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;
    [BindProperty]
    public string CategoryProduct { get; set; } = "Publicación libre";

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

    public async Task<ActionResult> OnGet(string ID)
    {
        try
        {
            await GetProduct(ID);
            await GetUserVentor(ProductSelected.IdVendor.ToString());
            await GetComments(ID);
            var category = await GetCategory(ProductSelected.IdCategory);
            CategoryProduct = string.IsNullOrEmpty(category) ? "Libre" : category;
            return Page();
        }
        catch (Exception)
        {
            return RedirectToPage("./Error", string.Empty, new { code = 500 });
        }

    }

    private async Task<Product> GetProduct(string productId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{productId}");
        if (response.IsSuccessStatusCode)
            ProductSelected = await response.Content.ReadAsAsync<Product>();
        return ProductSelected;
    }

    private async Task<string> GetCategory(int? id)
    {
        try
        {
            var client = GetPreparedClient("https://localhost:7278/api/Categories/detail/");
            HttpResponseMessage response = await client.GetAsync($"{id}");
            if (response.IsSuccessStatusCode)
                return CategoryProduct = (await response.Content.ReadAsAsync<Category>()).Name;
            else
                return string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private async Task<List<Comment>> GetComments(string idProduct)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Comments/");
        HttpResponseMessage response = await client.GetAsync($"{idProduct}");
        if (response.IsSuccessStatusCode)
            Comments = await response.Content.ReadAsAsync<List<Comment>>();
        return Comments;
    }

    private async Task<User> GetUserVentor(string userId)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Users/");
        HttpResponseMessage response = await client.GetAsync($"{userId}");
        if (response.IsSuccessStatusCode)
            UserVentor = await response.Content.ReadAsAsync<User>();
        return UserVentor;
    }

    public async Task<ActionResult> OnPostBuyProduct(string idProduct, string productQuantity)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                await GetProduct(idProduct);
                await GetUserVentor(ProductSelected.IdVendor.ToString());
                if (ProductSelected.StockQuantity < Convert.ToInt32(productQuantity))
                    throw new Exception("Lo sentimos, no hay suficiente cantidad en el stock del producto");
                return RedirectToPage("./ShopNote", "UnitProductShow", new { id = idProduct, quantity = productQuantity });
            }
            catch (Exception ex)
            {
                IsCorrect = "No";
                ErrorMessage = ex.Message;
                return Page();
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public async Task<IActionResult> OnPostAddShoppingCart(string idProduct, string productQuantity)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                await GetProduct(idProduct);
                await GetUserVentor(ProductSelected.IdVendor.ToString());
                if (ProductSelected.StockQuantity < Convert.ToInt32(productQuantity))
                    throw new Exception("Lo sentimos, no hay suficiente cantidad en el stock del producto");
                ProductSelected.StockQuantity = Convert.ToInt32(productQuantity);
                var idUser = HttpContext.Session.GetString("id");
                var client = GetPreparedClient("https://localhost:7278/api/ShoppingCart/");
                HttpResponseMessage response = await client.PostAsync
                (
                    $"{idUser}",
                    new StringContent(JsonConvert.SerializeObject(ProductSelected), Encoding.UTF8, "application/json")
                );
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Producto agregado al carrito", urlRedirection = "/ShoppingCart", urlTittle = "Ver carrito" });
                else
                    throw new Exception("500");
            }
            catch (Exception ex)
            {
                int code;
                if (int.TryParse(ex.Message, out code))
                {
                    return RedirectToPage("./Error", string.Empty, new { code = int.Parse(ex.Message) });
                }
                else
                {
                    IsCorrect = "No";
                    ErrorMessage = ex.Message;
                    return Page();
                }
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public async Task<IActionResult> OnGetAddFav(int idProduct)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                var idUser = HttpContext.Session.GetString("id");
                var client = GetPreparedClient("https://localhost:7278/api/Favorites/");
                HttpResponseMessage response = await client.PostAsync($"{idUser}/{idProduct}", null);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Producto agregado a favoritos", urlRedirection = "/Favorites", urlTittle = "Ver favoritos" });
                throw new Exception("500");
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Error", string.Empty, new { code = 500 });
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }
}
