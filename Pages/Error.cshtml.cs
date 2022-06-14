using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WaveShopClient.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private static string CommonMessage = "Si el problema persiste, favor de contactar a soporte técnico, comunicando el cpdigo y el ID del error.";
    public readonly Dictionary<int, string> Errors = new Dictionary<int, string>()
    {
        {401 , $"Lo sentimos, ocurrió un problema con la autentiación del usuario. {CommonMessage}"},
        {500, $"Lo sentimos, parece que el servidor no esta respondiendo correctamente. {CommonMessage}"},
        {400, $"Lo sentimos, la información ha sido actualizada reciententemente. {CommonMessage}"}
    };

    [BindProperty]
    public int Code { get; set; } = -1;
    [BindProperty]
    public string Message { get; set; } = string.Empty;

    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int code)
    {
        Code = code;
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        Message = string.Empty;
    }

    public void OnGetCustomError(int code, string message)
    {
        Code = code;
        Message = message;
    }

}

