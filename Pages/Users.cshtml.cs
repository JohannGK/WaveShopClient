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

public class UsersModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public List<User> Users { get; set; } = new List<User>();

    public UsersModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
        {
            if (HttpContext.Session.GetString("type") == "Administrador")
            {
                try
                {
                    var client = GetPreparedClient("https://localhost:7278/api/Users/");
                    HttpResponseMessage response = await client.GetAsync("");
                    if (response.IsSuccessStatusCode)
                    {
                        Users = await response.Content.ReadAsAsync<List<User>>();
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
                return RedirectToPage("./Permisses");
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
}