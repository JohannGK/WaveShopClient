using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class CategoryModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public Category Generic { get; set; } = new Category();
    [BindProperty]
    public string Type { get; set; } = "Add";

    public CategoryModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<ActionResult> OnGetAdd()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            return Page();
        }
        else
        {
            return RedirectToPage("./SignInRequire");
        }
    }

    public async Task<ActionResult> OnGetUpdate(string id)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                Type = "Update";
                Generic = await GetCategoryAsync(int.Parse(id));
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Error", string.Empty, new { code = int.Parse(ex.Message) });
            }
        }
        else
        {
            return RedirectToPage("./SignInRequire");
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

    public async Task<ActionResult> OnPostAdd(string name, string description)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                Category category = new Category(){
                    Id = -1,
                    Name = name,
                    Description = description,
                    Status = "Activa"
                };
                var client = GetPreparedClient("https://localhost:7278/api/Categories/");
                HttpResponseMessage response = await client.PostAsync(
                    $"",
                    new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json")
                );
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Categoría agregada exitosamente", urlRedirection = "/Categories", urlTittle = "Ver categorías" });
                throw new Exception();
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

    public async Task<ActionResult> OnPostUpdate(string id, string name, string description)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                Category category = await GetCategoryAsync(int.Parse(id));
                category.Name = name;
                category.Description = description;
                var client = GetPreparedClient("https://localhost:7278/api/Categories/");
                HttpResponseMessage response = await client.PutAsync(
                    $"{id}",
                    new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json")
                );
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Categoría actualizada exitosamente", urlRedirection = "/Categories", urlTittle = "Ver categorías" });
                throw new Exception();
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

    private async Task<Category> GetCategoryAsync(int id)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Categories/detail/");
        HttpResponseMessage response = await client.GetAsync($"{id}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<Category>();
        else
            return null;
    }
}