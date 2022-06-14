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
    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;

    public LoginModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            return RedirectToPage("./Profile");
        else
            return Page();
    }

    public void OnGetSignUp()
    {
        View = "SignUp";
    }

    private bool SetSession()
    {
        try
        {
            HttpContext.Session.Set("email", Encoding.UTF8.GetBytes(Client.Email));
            HttpContext.Session.Set("password", Encoding.UTF8.GetBytes(Client.Password));
            HttpContext.Session.Set("username", Encoding.UTF8.GetBytes(Client.UserName));
            HttpContext.Session.Set("id", Encoding.UTF8.GetBytes(Client.Id.ToString()));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IActionResult> OnPostSignInResponse(string email, string password)
    {
        try
        {
            var client = GetPreparedClient("https://localhost:7278/api/Users/mail/");
            HttpResponseMessage response = await client.GetAsync($"{email}");
            if (response.IsSuccessStatusCode)
            {
                Client = await response.Content.ReadAsAsync<User>();
                if (Client.Password == password)
                {
                    if (SetSession())
                        return RedirectToPage("./Index");
                    else
                        throw new Exception("401");
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
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    IsCorrect = "No";
                    Client.Email = email;
                    Client.Password = password;
                    return Page();
                }
                throw new Exception("500");
            }
        }
        catch (Exception ex)
        {
            return RedirectToPage("./Error", string.Empty, new { code = int.Parse(ex.Message) });
        }

    }

    private User GetObject(string userNameNew, string emailNew, string passwordNew, string phone, string birthDate, string isAdmin, string biography, string adminCode)
    {
        TimeSpan t = DateTime.Now.Subtract(Convert.ToDateTime(birthDate));
        User client = new User()
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
            Age = Convert.ToInt32(t.TotalDays / 365)
        };
        return client;
    }

    public async Task<IActionResult> OnPostSignUpResponse(string userNameNew, string emailNew, string passwordNew, string phone, string birthDate, string isAdmin, string biography, string adminCode)
    {
        try
        {
            View = "SignUp";
            Client = GetObject(userNameNew, emailNew, passwordNew, phone, birthDate, isAdmin, biography, adminCode);
            CheckUserData(Client);
            var client = GetPreparedClient("https://localhost:7278/api/Users/");
            HttpResponseMessage response = await client.PostAsync(
                $"",
                new StringContent(JsonConvert.SerializeObject(Client), Encoding.UTF8, "application/json")
            );
            if (response.IsSuccessStatusCode)
            {
                Client = await response.Content.ReadAsAsync<User>();
                if (SetSession())
                    return RedirectToPage("./Index");
                else
                    throw new Exception("401");
            }
            else
            {
                IsCorrect = "No";
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                ErrorMessage = json["value"]["error"].ToString();
                return Page();
            }
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

    private void CheckUserData(User user, string? adminCode = null)
    {
        if (user.Age < 18)
            throw new Exception("Para registrarte debes tener al menos 18 años");
        if (user.UserType == "Administrador")
            if (adminCode == null || adminCode != "1324")
                throw new Exception("Lo sentimos, el código de administrador no es correcto");
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