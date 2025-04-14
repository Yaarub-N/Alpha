using Business.Interfaces;
using Data.Contexts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[Authorize]
public class UsersController(IUserService userService, AppDbContext context) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly AppDbContext _context = context;


    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var result = await _userService.GetAllUsersAsync();

        if (!result.Succeeded || result.Result == null)
            return View(new List<User>());

        return View(result.Result);
    }


    [Route("admin / members")]
    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var users = await _context.Users
            .Where(x => x.FirstName!.Contains(term) || x.LastName!.Contains(term) || x.Email!.Contains(term))
            .Select(x => new { x.Id, FullName = $"{x.FirstName} {x.LastName}" })//x.Image
            .ToListAsync();

        return Json(users);
    }


}
