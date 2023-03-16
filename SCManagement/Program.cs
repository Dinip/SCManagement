using Microsoft.Extensions.Options;
using SCManagement;
using SCManagement.Middlewares;
using SCManagement.Services;

var builder = WebApplication.CreateBuilder(args);
RegisterServices.Register(builder.Configuration, builder.Services);

var app = builder.Build();

//sprint3 branch

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

app.UseWhen(context => context.Request.Path.StartsWithSegments("/MyClub") && !context.Request.Path.Value.Contains("Unavailable"), appBuilder =>
{
    appBuilder.UseClubMiddleware();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/Statistics"), appBuilder =>
{
    appBuilder.UseClubMiddleware();
});

using (var ser = app.Services.CreateScope())
{
    var services = ser.ServiceProvider;

    var localizationOptions = services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
    app.UseRequestLocalization(localizationOptions);
}
//this needs to be in this order, after user auth
app.UseRequestLocalizationCookies();


app.MapControllerRoute(
    name: "myclub",
    pattern: "MyClub/",
    defaults: new { controller = "MyClub", action = "Index" });

app.MapControllerRoute(
    name: "clubs",
    pattern: "Clubs/{id}",
    defaults: new { controller = "Clubs", action = "Index" },
    constraints: new { id = @"\d+" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


using var scope = app.Services.CreateScope();
await Configurations.CreateRoles(scope.ServiceProvider);

app.Run();
