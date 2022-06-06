using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class LoginModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public string View { get; set; } = "Login";
    [BindProperty]
    public User Client { get; set; } = new User();
    [BindProperty]
    public string IsCorrect { get; set; } = "Yes";
    [BindProperty]
    public string AdminCode { get; set; }
    [BindProperty]
    public string IsAdmin { get; set; } = "false";

    public LoginModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            return RedirectToPage("./Profile");
        else
            return Page();
    }

    public async Task OnGetSignUp()
    {
        View = "SignUp";
    }

    public async Task<IActionResult> OnPostSignInResponse(string email, string password)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Users/mail/");
        HttpResponseMessage response = await client.GetAsync($"{email}");
        if (response.IsSuccessStatusCode)
        {
            Client = await response.Content.ReadAsAsync<User>();
            if (Client.Password == password)
            {
                HttpContext.Session.Set("email", Encoding.UTF8.GetBytes(email));
                HttpContext.Session.Set("password", Encoding.UTF8.GetBytes(password));
                HttpContext.Session.Set("username", Encoding.UTF8.GetBytes(Client.UserName));
                HttpContext.Session.Set("id", Encoding.UTF8.GetBytes(Client.Id.ToString()));
                return RedirectToPage("./Index");
            }
            else
            {
                IsCorrect = "No";
                Client.Email = email;
                Client.Password = password;
                return Page();
            }
        }
        else
        {
            IsCorrect = "No";
            Client.Email = email;
            Client.Password = password;
            return Page();
        }

    }

    public async Task<IActionResult> OnPostSignUpResponse(string userNameNew, string emailNew, string passwordNew, string phone, string birthDate, string isAdmin, string biography, string adminCode)
    {
        View = "SignUp";
        Client = new User()
        {
            UserName = userNameNew,
            Email = emailNew,
            Password = passwordNew,
            Phone = phone,
            Status = "Active",
            BirthDay = Convert.ToDateTime(birthDate),
            UserType = isAdmin == "true" ? "Administrador" : "Usuario",
            Description = biography,
            Reputation = "Good",
            Age = 23

        };
        var client = GetPreparedClient("https://localhost:7278/api/Users/");
        HttpResponseMessage response = await client.PostAsync(
            $"",
            new StringContent(JsonConvert.SerializeObject(Client), Encoding.UTF8, "application/json")
        );
        if (response.IsSuccessStatusCode)
        {
            Client = await response.Content.ReadAsAsync<User>();
            HttpContext.Session.Set("email", Encoding.UTF8.GetBytes(Client.Email));
            HttpContext.Session.Set("password", Encoding.UTF8.GetBytes(Client.Password));
            HttpContext.Session.Set("username", Encoding.UTF8.GetBytes(Client.UserName));
            HttpContext.Session.Set("id", Encoding.UTF8.GetBytes(Client.Id.ToString()));
            return RedirectToPage("./Index");
        }
        else
        {
            IsCorrect = "No";
            return Page();
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