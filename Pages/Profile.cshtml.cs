using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ProfileModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public User Client { get; set; } = new User();
    [BindProperty]
    public string ViewType { get; set; } = "Request";

    public ProfileModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
        {
            var mail = HttpContext.Session.GetString("email");
            var client = GetPreparedClient("https://localhost:7278/api/Users/mail/");
            HttpResponseMessage response = await client.GetAsync($"{mail}");
            if (response.IsSuccessStatusCode)
                Client = await response.Content.ReadAsAsync<User>();
        }
    }

    public async Task OnPostEditProfile()
    {

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