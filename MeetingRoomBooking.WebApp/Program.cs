using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Manager;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.WebApp.Models;
using MeetingRoomBooking.Services.Managers;

var builder = WebApplication.CreateBuilder(args);

// Ensure the correct key path
var tokenAuthSection = builder.Configuration.GetSection("TokenAuth");
if (!tokenAuthSection.Exists()) {
    throw new InvalidOperationException("SecretKey section not configured in appsettings.json.");
}

PasswordManager.SetUp(tokenAuthSection);

// Add services to the container.
builder.Services.AddDbContext<MeetingRoomBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.")));

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<LoginManager>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.Run();
