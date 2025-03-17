
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using ZenlessZoneZeroWiki.Data;
using FirebaseAdmin;

var builder = WebApplication.CreateBuilder(args);

// Add Firebase services
FirebaseApp.Create(new AppOptions()
{
    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("zenlesszonezerowikiauth-firebase-adminsdk-fbsvc-8ea1214dac.json")
});

// Get connection string for database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ZenlessZoneZeroContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))
);

// 1. Register services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


var app = builder.Build();

// 3. Use environment-specific middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 4. Configure the HTTP pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapBlazorHub();

app.Run();
