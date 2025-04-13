    using Data.Extensions;
using Business.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Data.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddContexts(builder.Configuration.GetConnectionString("SqlConnection")!);
builder.Services.AddLocalIdentity(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddServices(builder.Configuration);






builder.Services.AddAuthentication(options =>
    {
   options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["ExtarnAuthentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["ExtarnAuthentication:Google:ClientSecret"]!;
        options.SignInScheme = IdentityConstants.ExternalScheme;

    }); 

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();

app.UseRewriter(new RewriteOptions().AddRedirect("^$","/admin/overview"));
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Overview}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
