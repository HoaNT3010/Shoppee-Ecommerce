using Microsoft.AspNetCore.Authentication.Cookies;
using ShoppeeEcommerce.MVC.Customer.Common;
using ShoppeeEcommerce.MVC.Customer.Configuration;
using ShoppeeEcommerce.MVC.Customer.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ForceLoginFilter>();
});
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
    });
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureRefit(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

//app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<CartSessionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
