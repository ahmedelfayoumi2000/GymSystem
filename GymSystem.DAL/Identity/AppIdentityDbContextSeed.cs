using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymSystem.DAL.Identity
{
    #region 
    //public static class AppIdentityDbContextSeed
    //{
    //    /// <summary>
    //    /// تهيئة الأدوار والمستخدمين
    //    /// </summary>
    //    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    //    {
    //        // تهيئة الأدوار
    //        await SeedRolesAsync(roleManager);

    //        // تهيئة المستخدمين
    //        await SeedAdminUsersAsync(userManager);
    //        await SeedTrainerUsersAsync(userManager);
    //        await SeedMemberUsersAsync(userManager);
    //        await SeedReceptionistUsersAsync(userManager);
    //    }

    //    /// <summary>
    //    /// تهيئة الأدوار
    //    /// </summary>
    //    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    //    {
    //        var roles = new List<string> { "Admin", "Trainer", "Member", "Receptionist" };

    //        foreach (var roleName in roles)
    //        {
    //            if (!await roleManager.RoleExistsAsync(roleName))
    //            {
    //                await roleManager.CreateAsync(new IdentityRole { Name = roleName });
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// تهيئة مستخدمي Admin
    //    /// </summary>
    //    private static async Task SeedAdminUsersAsync(UserManager<AppUser> userManager)
    //    {
    //        var adminUsers = new List<(AppUser user, string password)>
    //        {
    //            (
    //                new AppUser
    //                {
    //                    DisplayName = "Ahmed Admin",
    //                    UserName = "ahmedadmin",
    //                    Email = "ahmedelfayoumi203@gmail.com",
    //                    PhoneNumber = "01093422098",
    //                    Address = new Address
    //                    {
    //                        FristName = "Ahmed",
    //                        LastName = "Elfayoumi",
    //                        Country = "Egypt",
    //                        City = "El-bagour",
    //                        Street = "10 Tahrir St."
    //                    },
    //                    UserCode = GenerateUserCode("Admin", 1)
    //                },
    //                "Pa$$w0rd"
    //            )
    //        };

    //        await SeedUsersWithRole(userManager, adminUsers, "Admin");
    //    }

    //    /// <summary>
    //    /// تهيئة مستخدمي Trainer
    //    /// </summary>
    //    private static async Task SeedTrainerUsersAsync(UserManager<AppUser> userManager)
    //    {
    //        var trainerUsers = new List<(AppUser user, string password)>
    //        {
    //            (
    //                new AppUser
    //                {
    //                    DisplayName = "Mohamed Trainer",
    //                    UserName = "mohamedtrainer",
    //                    Email = "mohamedtrainer@example.com",
    //                    PhoneNumber = "0123456789",
    //                    Address = new Address
    //                    {
    //                        FristName = "Mohamed",
    //                        LastName = "Trainer",
    //                        Country = "Egypt",
    //                        City = "Cairo",
    //                        Street = "15 Nile St."
    //                    },
    //                    UserCode = GenerateUserCode("Trainer", 1)
    //                },
    //                "Trainer@123"
    //            )
    //        };

    //        await SeedUsersWithRole(userManager, trainerUsers, "Trainer");
    //    }

    //    /// <summary>
    //    /// تهيئة مستخدمي Member
    //    /// </summary>
    //    private static async Task SeedMemberUsersAsync(UserManager<AppUser> userManager)
    //    {
    //        var memberUsers = new List<(AppUser user, string password)>
    //        {
    //            (
    //                new AppUser
    //                {
    //                    DisplayName = "Ali Member",
    //                    UserName = "alimember",
    //                    Email = "alimember@example.com",
    //                    PhoneNumber = "0112233445",
    //                    Address = new Address
    //                    {
    //                        FristName = "Ali",
    //                        LastName = "Member",
    //                        Country = "Egypt",
    //                        City = "Alexandria",
    //                        Street = "20 Corniche St."
    //                    },
    //                    UserCode = GenerateUserCode("Member", 1)
    //                },
    //                "Member@123"
    //            )
    //        };

    //        await SeedUsersWithRole(userManager, memberUsers, "Member");
    //    }

    //    /// <summary>
    //    /// تهيئة مستخدمي Receptionist
    //    /// </summary>
    //    private static async Task SeedReceptionistUsersAsync(UserManager<AppUser> userManager)
    //    {
    //        var receptionistUsers = new List<(AppUser user, string password)>
    //        {
    //            (
    //                new AppUser
    //                {
    //                    DisplayName = "Sara Receptionist",
    //                    UserName = "sarareceptionist",
    //                    Email = "sarareceptionist@example.com",
    //                    PhoneNumber = "0109876543",
    //                    Address = new Address
    //                    {
    //                        FristName = "Sara",
    //                        LastName = "Receptionist",
    //                        Country = "Egypt",
    //                        City = "Giza",
    //                        Street = "25 Pyramid St."
    //                    },
    //                    UserCode = GenerateUserCode("Receptionist", 1)
    //                },
    //                "Receptionist@123"
    //            )
    //        };

    //        await SeedUsersWithRole(userManager, receptionistUsers, "Receptionist");
    //    }

    //    /// <summary>
    //    /// إضافة المستخدمين مع أدوارهم
    //    /// </summary>
    //    private static async Task SeedUsersWithRole(UserManager<AppUser> userManager, List<(AppUser user, string password)> users, string role)
    //    {
    //        foreach (var (user, password) in users)
    //        {
    //            if (await userManager.FindByEmailAsync(user.Email) == null)
    //            {
    //                var result = await userManager.CreateAsync(user, password);

    //                if (result.Succeeded)
    //                {
    //                    await userManager.AddToRoleAsync(user, role);
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// توليد رمز المستخدم الفريد
    //    /// </summary>
    //    private static string GenerateUserCode(string role, int userCount)
    //    {
    //        return $"{role.Substring(0, 2).ToUpper()}-{DateTime.UtcNow.ToString("yyMMdd")}-{userCount}";
    //    }
    //}
    #endregion
}