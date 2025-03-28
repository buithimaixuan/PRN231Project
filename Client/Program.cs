﻿using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

//TĂNG DUNG LƯỢNG FILE KHI GỬI QUA FORM
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100000000; // 100MB
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache(); // For storing session data in memory
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CookiesPRN231";
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Sử dụng Google cho challenge
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Thêm DefaultSignInScheme
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "CookiesPRN231";
    options.Cookie.HttpOnly = true; // Bảo vệ chống XSS
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ gửi cookie qua HTTPS
    options.LoginPath = "/Authen/Index"; // Trang đăng nhập
    options.AccessDeniedPath = "/Authen/AccessDenied"; // Trang bị từ chối quyền
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút
    options.SlidingExpiration = true; // Gia hạn nếu có hoạt động
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientID"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
app.UseRouting();


app.UseSession();

app.UseAuthentication();

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();