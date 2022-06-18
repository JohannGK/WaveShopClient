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

public class UserDataModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public User Generic { get; set; } = new User();
    [BindProperty]
    public string UserType { get; set; } = "Client";

    public UserDataModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<ActionResult> OnGet(string id)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                UserType = HttpContext.Session.GetString("type");
                var client = GetPreparedClient("https://localhost:7278/api/Users/");
                HttpResponseMessage response = await client.GetAsync($"{id}");
                if (response.IsSuccessStatusCode)
                {
                    Generic = await response.Content.ReadAsAsync<User>();
                    return Page();
                }
                else
                {
                    throw new Exception();
                }
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

    public async Task<ActionResult> OnPostBanned(string id)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                User user = await GetClientAsync(int.Parse(id));
                user.Status = "Banned";
                user.Reputation = "Bad";
                var client = GetPreparedClient("https://localhost:7278/api/Users/");
                HttpResponseMessage response = await client.PutAsync(
                    $"{id}",
                    new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
                );
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Usuario dado de baja", urlRedirection = "/Users", urlTittle = "Ver usuarios" });
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

    public async Task<ActionResult> OnPostActive(string id)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            try
            {
                User user = await GetClientAsync(int.Parse(id));
                user.Status = "Activo";
                user.Reputation = "Good";
                var client = GetPreparedClient("https://localhost:7278/api/Users/");
                HttpResponseMessage response = await client.PutAsync(
                    $"{id}",
                    new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
                );
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Success", string.Empty, new { header = "Usuario dado de alta", urlRedirection = "/Users", urlTittle = "Ver usuarios" });
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

    private async Task<User> GetClientAsync(int id)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Users/");
        HttpResponseMessage response = await client.GetAsync($"{id}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<User>();
        return null;
    }
}