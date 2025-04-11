using Business.Interfaces;
using Business.Models;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    [Route("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var result = await _clientService.GetClientsAsync();

        if (!result.Succeeded)
        {
            return View(new List<Client>());
        }

        return View(result.Result);
    }



    [Route("admin/clients")]
    [HttpPost]
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

        var client = new Client
        {
            ClientName = form.ClientName,
            Email = form.Email,
            Phone = form.Phone!,
            Location = form.Location!
        };

        var result = await _clientService.AddClientAsync(client);

        if (!result.Succeeded)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(new { success = true });
    }

}
