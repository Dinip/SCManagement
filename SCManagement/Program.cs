using Microsoft.Extensions.Options;
using SCManagement;
using SCManagement.Services;
using SCManagement.Services.Location;

var builder = WebApplication.CreateBuilder(args);
RegisterServices.Register(builder.Configuration, builder.Services);

builder.Services.AddTransient<ILocationService, LocationService>();

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

using (var ser = app.Services.CreateScope())
{
    var services = ser.ServiceProvider;

    var localizationOptions = services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
    app.UseRequestLocalization(localizationOptions);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Address",
    pattern: "{controller=Addresses}/{action=Create}/{address}/{addressComponent?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using var scope = app.Services.CreateScope();
await Configurations.CreateRoles(scope.ServiceProvider);

app.Run();
