    using Data.Extensions;
using Business.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddContexts(builder.Configuration.GetConnectionString("SqlConnection")!);
builder.Services.AddLocalIdentity(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddServices(builder.Configuration);












//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
//builder.Services.AddIdentity<UserEntity, IdentityRole>(x =>
//{
//    x.User.RequireUniqueEmail = true;
//    x.Password.RequiredLength = 8;
//})
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/auth/login";
//    options.AccessDeniedPath = "/auth/denied";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//    options.ExpireTimeSpan = TimeSpan.FromDays(30);
//    options.SlidingExpiration = true;
//}); 

//builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IStatusRepository, StatusRepository>();
//builder.Services.AddScoped<IClientRepository, ClientRepository>();


//builder.Services.AddScoped<IProjectService, ProjectService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IStatusService, StatusService>();
//builder.Services.AddScoped<IClientService, ClientService>();
//builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/admin/overview"));
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
