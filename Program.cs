using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using WebUniversity.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DBContext>(options =>
{
    var configuration = builder.Configuration; // Lấy trực tiếp từ builder
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseLazyLoadingProxies().UseSqlServer(connectionString);
});
builder.Host.UseSerilog((context, config) =>
{
    config
        // Chỉ ghi log KHÔNG đến từ Microsoft hoặc System
        .Filter.ByExcluding(logEvent =>
            logEvent.Properties.ContainsKey("SourceContext") &&
            (
                logEvent.Properties["SourceContext"].ToString().StartsWith("\"Microsoft") ||
                logEvent.Properties["SourceContext"].ToString().StartsWith("\"System")
            )
        )
        .MinimumLevel.Information() // Tùy chọn mức log tối thiểu
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddHostedService<BackupService>();

// Thêm services cho Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
        policy.RequireAuthenticatedUser()); // Policy mặc định yêu cầu user đã đăng nhập
});

builder.Services.AddSingleton<IAuthorizationHandler, AdminAccessHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotLectureOrStudent", policy =>
        policy.RequireAssertion(context =>
            !context.User.IsInRole("LectureRole") &&
            !context.User.IsInRole("StudentRole"))); // Chặn cả hai vai trò
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/Admin")
    {
        context.Response.Redirect("/Admin/Dashboard");
        return;
    }
    else if (context.Request.Path == "/Lecturer")
    {
        context.Response.Redirect("/Lecturer/Info");
        return;
    }
    else if (context.Request.Path == "/Student")
    {
        context.Response.Redirect("/Student/Info");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        var user = context.User;

        if (!user.Identity!.IsAuthenticated)
        {
            // Nếu chưa đăng nhập, chuyển hướng đến trang Login
            context.Response.Redirect("/Account/Login");
            return;
        }

        // Kiểm tra quyền để chuyển hướng đến đúng trang
        string redirectUrl = "/Admin"; // Mặc định là trang Admin

        if (user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "LecturerRole"))
        {
            redirectUrl = "/Lecturer";
        }
        else if (user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "StudentRole"))
        {
            redirectUrl = "/Student";
        }

        context.Response.Redirect(redirectUrl);
        return;
    }

    await next();
});

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DBContext>();
    try
    {
        if (context.Database.CanConnect())
        {
            Log.Information("✅ Đã kết nối được đến database");
        }
        else
        {
            Log.Error("❌ Không thể kết nối đến database");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Không thể kết nối đến database (exception)");
    }
}

app.Run();
