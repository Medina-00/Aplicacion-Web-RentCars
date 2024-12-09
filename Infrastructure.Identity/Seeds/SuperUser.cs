

using Core.Application.Enums;
using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Infrastructure.Identity.Seeds
{
    public static class SuperUser
    {
        public static async Task SeedAsync(UserManager<ApplitationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //CREDENCIALES DEL USUARIO GENERAL
            ApplitationUser defaultUser = new();
            defaultUser.Nombre = "Hola";
            defaultUser.Apellido = "Mundo";
            defaultUser.UserName = "SuperAdmin01";
            defaultUser.Email = "superadminuser@email.com";
            defaultUser.EmailConfirmed = true;
            defaultUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "HolaMundo12*");
                    await userManager.AddToRoleAsync(defaultUser, EnumRoles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, EnumRoles.Usuario.ToString());
                }
            }

        }

    }
}
