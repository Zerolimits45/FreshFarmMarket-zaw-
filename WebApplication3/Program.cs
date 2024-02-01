
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 3;
})
    .AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddDataProtection();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.ConfigureApplicationCookie(Config => // cookie options, ensure that the path is set to login page
{
    Config.Cookie.HttpOnly = true;
    Config.LoginPath = "/Login";
    Config.ExpireTimeSpan = TimeSpan.FromSeconds(10);
    Config.SlidingExpiration = true;
    Config.Cookie.SameSite = SameSiteMode.Strict;

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/Errors/{0}");

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
