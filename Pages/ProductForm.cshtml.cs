using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaveShopClient;
using WaveShopClient.Pages;
using WaveShopClient.Pages.Model;

public class ProductFormModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public Product ProductToSell { get; set; } = new Product();
    [BindProperty]
    public string View { get; set; }
    [BindProperty]
    public List<IFormFile> FilesToUpload { get; set; }
    [BindProperty]
    public string IsCorrect { get; set; } = "Yes";
    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;
    [BindProperty]
    public string Status { get; set; } = "Visible";
    [BindProperty]
    public List<Category> Categories { get; set; } = new List<Category>();
    private string PathRoute { get; set; }
    private readonly IWebHostEnvironment webHostEnvironment;

    public ProductFormModel(IWebHostEnvironment webHostEnvironment)
    {
        this.webHostEnvironment = webHostEnvironment;
    }

    public async Task<ActionResult> OnGet()
    {
        try
        {
            View = "Form";
            await GetCategories();
            return Page();
        }
        catch (Exception)
        {
            return RedirectToPage("./Error", string.Empty, new { code = 500 });
        }
    }

    public async Task<ActionResult> OnGetEdit(string idProduct)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                View = "FormUpdate";
                await GetCategories();
                ProductToSell = await GetProduct(idProduct);
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
        var client = GetPreparedClient("https://localhost:7278/api/Products/");
        HttpResponseMessage response = await client.GetAsync($"{productId}");
        if (response.IsSuccessStatusCode)
            ProductToSell = await response.Content.ReadAsAsync<Product>();
        return ProductToSell;
    }

    public async Task<IActionResult> OnPostUpdate(string ID, List<IFormFile> filesList, string productName, string location, string country, string unitPrice, string stock, string status, string description, int category)
    {
        try
        {
            ProductToSell = await GetProduct(ID);
            var images = await UploadImages(filesList);
            ProductToSell.Name = productName;
            ProductToSell.Location = location;
            ProductToSell.Country = country;
            ProductToSell.IdCategory = category;
            ProductToSell.UnitPrice = double.Parse(unitPrice);
            ProductToSell.StockQuantity = int.Parse(stock);
            ProductToSell.Status = status;
            ProductToSell.Description = description;
            var client = GetPreparedClient("https://localhost:7278/api/Products/");
            HttpResponseMessage response = await client.PutAsync(
                $"{ProductToSell.Id}",
                new StringContent(JsonConvert.SerializeObject(ProductToSell), Encoding.UTF8, "application/json")
            );
            CleanDirectory(PathRoute);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Success", string.Empty, new { header = "Producto actualizado", urlRedirection = "/ProductsSold", urlTittle = "Ver productos vendidos" });
            IsCorrect = "No";
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            ErrorMessage = json["value"]["error"].ToString();
            throw new Exception(ErrorMessage);
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

    private Product GetObject(string productName, string location, string country, string unitPrice, string stock, string status, string description, int category)
    {
        Product product = new Product()
        {
            Id = -1,
            Name = productName,
            Description = description,
            VideoAddress = null,
            StockQuantity = int.Parse(stock),
            UnitPrice = double.Parse(unitPrice),
            Status = status,
            Published = DateTime.Now,
            Country = country,
            Location = location,
            IdCategory = category,
            IdVendor = int.Parse(HttpContext.Session.GetString("id")),
            LikesNumber = 0,
            DislikesNumber = 0,
            ShoppedTimes = 0,
            CommentsNumber = 0,
            LastUpdate = DateTime.Now,
            VendorUsername = HttpContext.Session.GetString("username")
        };
        return product;
    }

    public async Task<IActionResult> OnPost(List<IFormFile> filesList, string productName, string location, string country, string unitPrice, string stock, string status, string description, int category)
    {
        try
        {
            View = "Form";
            ProductToSell = GetObject(productName, location, country, unitPrice, stock, status, description, category);
            if (filesList == null || filesList.Count <= 0)
                throw new Exception("Debes subir un archivo");
            View = "Success";
            var images = await UploadImages(filesList);
            ProductToSell.Product_Images = images.ToArray();
            var client = GetPreparedClient("https://localhost:7278/api/Products/");
            HttpResponseMessage response = await client.PostAsync(
                $"",
                new StringContent(JsonConvert.SerializeObject(ProductToSell), Encoding.UTF8, "application/json")
            );
            CleanDirectory(PathRoute);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Success", string.Empty, new { header = "Producto registrado", urlRedirection = "/ProductsSold", urlTittle = "Ver productos vendidos" });
            IsCorrect = "No";
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            ErrorMessage = json["value"]["error"].ToString();
            throw new Exception(ErrorMessage);
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

    private async Task<List<Product_Image>?> UploadImages(List<IFormFile> files)
    {
        List<Product_Image> images = new List<Product_Image>();
        if (files != null && files.Count > 0)
        {
            foreach (IFormFile item in files)
            {
                PathRoute = Path.Combine(webHostEnvironment.WebRootPath, "Upload");
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Upload", item.FileName);
                var stream = new FileStream(path, FileMode.Create);
                await item.CopyToAsync(stream);
                var response = await GoogleDriveApi.CargarImagen(stream);
                images.Add(new Product_Image()
                {
                    Id = -1,
                    Url = response,
                    LastUpdate = DateTime.Now,
                    IdProduct = -1
                });
            }
            return images;
        }
        else
        {
            return null;
        }
    }

    private void CleanDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
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

    private async Task<List<Category>> GetCategories()
    {
        var client = GetPreparedClient("https://localhost:7278/api/Categories/");
        HttpResponseMessage response = await client.GetAsync($"");
        if (response.IsSuccessStatusCode)
            return Categories = await response.Content.ReadAsAsync<List<Category>>();
        throw new Exception();
    }
}
