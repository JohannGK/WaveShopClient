using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaveShopClient.Pages.Model;

namespace WaveShopClient.Pages;

public class ProfileModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty]
    public User Client { get; set; } = new User();
    [BindProperty]
    public string ViewType { get; set; } = "Request";
    [BindProperty]
    public string IsCorrect { get; set; } = "Yes";
    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;

    public ProfileModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
        {
            var id = HttpContext.Session.GetString("id");
            try
            {
                await GetClientAsync(int.Parse(id));
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

    public async Task<ActionResult> OnPostEditProfile(string ID, string userName, string email, string password, string phone, string birthDate, string biography)
    {
        try
        {
            Client = await GetClientAsync(int.Parse(ID));
            GetObject(userName, email, password, phone, birthDate, biography);
            CheckUserData(Client);
            var client = GetPreparedClient("https://localhost:7278/api/Users/");
            HttpResponseMessage response = await client.PutAsync(
                $"{Client.Id}",
                new StringContent(JsonConvert.SerializeObject(Client), Encoding.UTF8, "application/json")
            );
            if (response.IsSuccessStatusCode)
            {
                Client = await response.Content.ReadAsAsync<User>();
                if (SetSession())
                    return RedirectToPage("./Success", string.Empty, new { header = "Tu perfil se ha actualizado!", urlRedirection = "/Index", urlTittle = "Inicio" });
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

    private User GetObject(string userName, string email, string password, string phone, string birthDate, string biography)
    {
        Client.UserName = userName;
        Client.Email = email;
        Client.Password = password;
        Client.Phone = phone;
        Client.BirthDay = Convert.ToDateTime(birthDate);
        Client.Description = biography;
        return Client;
    }

    private bool SetSession()
    {
        try
        {
            HttpContext.Session.Set("email", Encoding.UTF8.GetBytes(Client.Email));
            HttpContext.Session.Set("password", Encoding.UTF8.GetBytes(Client.Password));
            HttpContext.Session.Set("username", Encoding.UTF8.GetBytes(Client.UserName));
            HttpContext.Session.Set("id", Encoding.UTF8.GetBytes(Client.Id.ToString()));
            HttpContext.Session.Set("type", Encoding.UTF8.GetBytes(Client.UserType.ToString()));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void CheckUserData(User user)
    {
        TimeSpan t = DateTime.Now.Subtract(Convert.ToDateTime(user.BirthDay));
        if (Convert.ToInt32(t.TotalDays / 365) < 18)
            throw new Exception("Para registrarte debes tener al menos 18 años");
    }


    private async Task<User> GetClientAsync(int id)
    {
        var client = GetPreparedClient("https://localhost:7278/api/Users/");
        HttpResponseMessage response = await client.GetAsync($"{id}");
        if (response.IsSuccessStatusCode)
            Client = await response.Content.ReadAsAsync<User>();
        return Client;
    }

    public ActionResult OnGetLogOut()
    {
        HttpContext.Session.Set("email", Encoding.UTF8.GetBytes(string.Empty));
        HttpContext.Session.Set("password", Encoding.UTF8.GetBytes(string.Empty));
        HttpContext.Session.Set("username", Encoding.UTF8.GetBytes(string.Empty));
        HttpContext.Session.Set("id", Encoding.UTF8.GetBytes(string.Empty));
        HttpContext.Session.Set("type", Encoding.UTF8.GetBytes(string.Empty));
        return RedirectToPage("./Index");
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