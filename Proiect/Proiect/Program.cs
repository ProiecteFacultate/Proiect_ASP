using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Security.Policy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Show Profile",
    pattern: "Profile/Show/{id}",
    defaults: new { controller = "Profile", action = "Show" });

app.MapControllerRoute(
    name: "Edit Profile",
    pattern: "Profile/Edit/{id}",
    defaults: new { controller = "Profile", action = "Edit" });

app.MapControllerRoute(
    name: "Edit Profile Post",
    pattern: "Profile/Edit/{id}/{updatedProfile}",
    defaults: new { controller = "Profile", action = "Edit" });

app.MapControllerRoute(      //this is because when you log out it send you to Home/Index, but we replaced Home/Index to Home/Feed in the project
    name: "Home Index",
    pattern: "Home/Index",
    defaults: new { controller = "Home", action = "Feed" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Feed}/{id?}");

app.MapRazorPages();

app.Run();
