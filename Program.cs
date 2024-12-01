using App.ExtendMethods;
using App.Menu;
using App.Models;
using App.Models.UserManagement;
using App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();//Đăng ký các dịch vụ liên quan đến trang razor

//Kết nối SQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectString = builder.Configuration.GetConnectionString("AppConnectionString");
    options.UseSqlServer(connectString);
});
//Cấu hình cho gửi mail
builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSettings");// đọc config
builder.Services.Configure<MailSettings>(mailSetting);// đăng ký để Inject
builder.Services.AddSingleton<IEmailSender, SendMailService>();// Đăng ký dịch vụ Mail

//muốn đăng ký dịch vụ AdminSidebarService phải đăng ký IActionContextAccessor
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<AdminSidebarService>();

//Thay thế các thông báo lỗi mặc định của Identity
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();


//Dang ky Identity su dung giao dien tuy chinh sau khi generator từ Identity.UI (dotnet aspnet-codegenerator identity -dc App.Models.AppDbContext)
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user - Thiết lập số lần đăng nhập thất bại
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    // options.SignIn.RequireConfirmedAccount = true;
});

//Cấu hình cho Authorization
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/khongduoctruycap.html"; // nếu không cầu hình, đường dẫn mặc định sẽ là: /Identity/Account/AccessDenied
});

//Cấu hình đăng nhập bằng google
builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    // Đọc thông tin Authentication:Google từ appsettings.json
                    var ggconfiguration = builder.Configuration.GetSection("Authentication:Google");

                    // Thiết lập ClientID và ClientSecret để truy cập API google
                    googleOptions.ClientId = ggconfiguration["ClientId"];
                    googleOptions.ClientSecret = ggconfiguration["ClientSecret"];
                    // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
                    googleOptions.CallbackPath = "/dang-nhap-tu-google";

                });


var app = builder.Build();

// Thêm đoạn cấu hình CookiePolicyOptions để đăng nhập gg nếu không sẽ lỗi
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();//truy cập file tĩnh

// //cấu hình quản lý thư mục Uploads
// app.UseStaticFiles(new StaticFileOptions()
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
//     ),
//     RequestPath = "/uploads"
// });

// app.UseSession();//Kích hoạt session

//Tuy bien reponse từ lỗi 404-599 (tao trong /ExtendMethods/AppExtends.cs)
app.AddStatusCodePages();

app.UseRouting();

app.UseAuthentication();//Xác định danh tính
app.UseAuthorization();//Xác thực quyền truy cập



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();  //cho phép truy cập Razor Pages

app.Run();
