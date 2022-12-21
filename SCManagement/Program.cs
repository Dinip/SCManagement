using Auth.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SCManagement;
using SCManagement.Data;
using SCManagement.Models;

var builder = WebApplication.CreateBuilder(args);

//to use azure key vault for secrets instead of local secrets.json or appsettings.json
//disabled in development
//builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"]), new DefaultAzureCredential());

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

//add email service, in development it uses the local smtp server from mail catcher
//in production it will use mailgun with the api key stored in the vault
builder.Services.Configure<AuthMessageSenderOptions>(options => options.MailgunKey = builder.Configuration["MailgunKey"]);
builder.Services.AddTransient<IEmailSender, EmailSenderMailcatcher>();
//builder.Services.AddTransient<IEmailSender, EmailSenderMailgun>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using var scope = app.Services.CreateScope();
await Configurations.CreateRoles(scope.ServiceProvider);

app.Run();
