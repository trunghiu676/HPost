using App.Models;
using App.Models.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Kết nối SQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectString = builder.Configuration.GetConnectionString("AppConnectionString");
    options.UseSqlServer(connectString);
});

//Dang ky Identity su dung giao dien tuy chinh sau khi generator từ Identity.UI (dotnet aspnet-codegenerator identity -dc netEf.Models.MyBlogContext)
// builder.Services.AddIdentity<AppUser, IdentityRole>()
//                 .AddEntityFrameworkStores<AppDbContext>()
//                 .AddDefaultTokenProviders();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();//truy cập file tĩnh

app.UseRouting();

app.UseAuthentication();//Xác định danh tính
app.UseAuthorization();//Xác thực quyền truy cập



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
app.MapRazorPages();  //cho phép truy cập Razor Pages

app.Run();
