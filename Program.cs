using BTLWNCao.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext với Connection String từ appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// Đăng ký IHttpContextAccessor và Session
builder.Services.AddHttpContextAccessor();
// Cấu hình Session - sử dụng RAM
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true; // Ngăn không cho truy cập từ JavaScript
    options.Cookie.IsEssential = true; // Bắt buộc phải có (cho GDPR)
});

// Thêm dịch vụ MVC (Controllers + Views) và cấu hình JSON options
builder
    .Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System
            .Text
            .Json
            .Serialization
            .ReferenceHandler
            .Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

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

// Kích hoạt Session Middleware (phải đặt trước Authorization)
app.UseSession();

app.UseAuthorization();

// Cấu hình default route
app.MapControllerRoute(name: "default", pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
