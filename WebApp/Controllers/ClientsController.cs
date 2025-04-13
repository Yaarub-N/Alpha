using Business.Interfaces;
using Business.Models;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    [HttpGet("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var result = await _clientService.GetClientsAsync();
        return View(result.Succeeded ? result.Result : new List<Client>());
    }
    [HttpPost("admin/clients")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddClientForm form)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        // 👇 Kontrollera om email redan finns
        var email = form.Email.ToLower();
        if (!string.IsNullOrEmpty(email))
        {
            var existing = (await _clientService.GetClientsAsync()).Result!
                .FirstOrDefault(c => c.Email.ToLower() == form.Email.ToLower());


            if (existing is not null)
            {
                return BadRequest(new
                {
                    errors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "Email address already exists." } }
            }
                });
            }
        }

        var client = new Client
        {
            ClientName = form.ClientName,
            Email = form.Email,
            Phone = form.Phone,
            Location = form.Location
        };

        var result = await _clientService.AddClientAsync(client);

        if (!result.Succeeded)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return PartialView("~/Views/Shared/ListItems/_ClientListItemPartial.cshtml", client);
    }

}
