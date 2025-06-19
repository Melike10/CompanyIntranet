using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using CompanyIntranet.Models;

namespace CompanyIntranet.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roles = { "IK", "Calisan", "Yonetici" };

            // Roller ekleniyor
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Kullanıcılar ekleniyor
            var users = new[]
            {
                new { FullName = "Ayşe İnsanKaynakları", Email = "ayse@intranet.com", Role = "IK" },
                new { FullName = "Ali Çalışan", Email = "ali@intranet.com", Role = "Calisan" },
                new { FullName = "Zeynep Yönetici", Email = "zeynep@intranet.com", Role = "Yonetici" },
                new { FullName = "Melike IKDirektörü", Email = "demirmelike187@gmail.com", Role = "IK" }
            };

            foreach (var u in users)
            {
                var user = await userManager.FindByEmailAsync(u.Email);
                if (user == null)
                {
                    var newUser = new AppUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FullName = u.FullName,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(newUser, "P@ssword123");
                    await userManager.AddToRoleAsync(newUser, u.Role);
                }
            }
        }
    }
}
