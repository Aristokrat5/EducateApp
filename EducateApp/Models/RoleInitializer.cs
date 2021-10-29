using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Models
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {

            string adminEmail = "shitik.shitik@mail.ru";
            string password = "123456Aa";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("registeredUser") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("registeredUser"));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    LastName = "Шитик",
                    FirstName = "Николай",
                    Patronymic = "Александрович",
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}