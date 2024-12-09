

using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Core.Application.Enums;

namespace Infrastructure.Identity.Seeds
{
    public static class Roles
    {
        public static async Task SeedAsync(UserManager<ApplitationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(EnumRoles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(EnumRoles.Usuario.ToString()));
        }

    }
}
