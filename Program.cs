using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using Microsoft.AspNetCore.Identity;
using QuanLyHoatDongNgoaiKhoa.Hubs;
using QuanLyHoatDongNgoaiKhoa.Services;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình encoding UTF-8 cho tiếng Việt
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("vi-VN");
    options.SupportedCultures = new List<System.Globalization.CultureInfo> { new System.Globalization.CultureInfo("vi-VN") };
    options.SupportedUICultures = new List<System.Globalization.CultureInfo> { new System.Globalization.CultureInfo("vi-VN") };
});

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddMvc();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IScheduleOptimizationService, ScheduleOptimizationService>();

// CHỈ GIỮ LẠI 1 CẤU HÌNH IDENTITY DUY NHẤT
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

// Map SignalR hub cho tất cả environments
app.MapHub<HubThongBao>("/hubThongBao");

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Cấu hình localization cho tiếng Việt
app.UseRequestLocalization();

// ĐẢM BẢO THỨ TỰ ĐÚNG
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // CẦN THIẾT CHO IDENTITY

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}