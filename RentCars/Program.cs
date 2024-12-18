using Infrastructure.Identity;
using Core.Application;
using Infrastructure.Shared;
using Infrastructure.Persistence;
using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity.Seeds;
using OfficeOpenXml;  // Agregar la librería EPPlus para establecer el contexto de licencia.

var builder = WebApplication.CreateBuilder(args);

// Establecer el contexto de licencia de EPPlus a No Comercial (LGPL)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddPersistenceInfrastructure(builder.Configuration);



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplitationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await Roles.SeedAsync(userManager, roleManager);
        await SuperUser.SeedAsync(userManager, roleManager);


    }
    catch (Exception ex) { Console.WriteLine(ex); }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//(OJO) EN ESTE PARTE SIEMPRE HAY QUE ASEGURARSE QUE PRIMERO SE AUTHENTIQUE Y DESPUES AUTORIZE.
app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
