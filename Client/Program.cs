var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
//new
builder.Services.AddHttpClient();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache(); // For storing session data in memory
builder.Services.AddHttpContextAccessor();

// Thêm Authentication với Cookie
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CookiesPRN231";
})
.AddCookie("CookiesPRN231", options =>
{
    options.Cookie.Name = "CookiesPRN231";
    options.LoginPath = "/Authen/Index"; // Chuyển hướng đến trang đăng nhập
    options.AccessDeniedPath = "/Authen/AccessDenied"; // Chuyển hướng khi bị từ chối quyền
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn
    options.SlidingExpiration = true; // Gia hạn session khi có hoạt động
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
