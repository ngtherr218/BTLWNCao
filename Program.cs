using Microsoft.EntityFrameworkCore;
using BTLWNCao.Models;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext vào DI container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ⚡ Thêm dịch vụ session
builder.Services.AddDistributedMemoryCache(); // Lưu session tạm trong RAM
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚡ Bật middleware session
app.UseSession();

app.UseAuthorization();

// Cấu hình routing cho controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
