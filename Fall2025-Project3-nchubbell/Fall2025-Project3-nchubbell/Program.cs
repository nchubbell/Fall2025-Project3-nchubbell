using Fall2025_Project3_nchubbell.Data;
using Fall2025_Project3_nchubbell.Models;
using Fall2025_Project3_nchubbell.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Dependency Injection
// -----------------------------

// AI Review Service
builder.Services.AddScoped<IAIReviewService, AzureAIReviewService>();

// (Optional) Azure OpenAI config
var azureOpenAiSection = builder.Configuration.GetSection("AzureOpenAI");
builder.Services.Configure<AzureOpenAISettings>(azureOpenAiSection);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// MVC / Razor
builder.Services.AddControllersWithViews();

// -----------------------------
// Build App
// -----------------------------

var app = builder.Build();

// -----------------------------
// Middleware Pipeline
// -----------------------------

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// -----------------------------
// Routing
// -----------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
