using Microsoft.AspNetCore.Identity;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enums;

namespace ToDoListApp.DAL
{
    public class DataSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRoles> _roleManager;

        public DataSeeder(UserManager<User> userManager, RoleManager<UserRoles> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync(Roles.Admin.ToString()))
            {
                var adminRole = new UserRoles { Name = Roles.Admin.ToString() };
                await _roleManager.CreateAsync(adminRole);
            }
        }

        public async Task CreateUsers(List<User> users, Roles role)
        {
            foreach (var user in users)
            {
                var result = await _userManager.CreateAsync(user, "Admin123$");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role.ToString());
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminUser = await _userManager.FindByNameAsync(Roles.Admin.ToString());
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = Roles.Admin.ToString()
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123$");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                }
                else
                {
                    throw new Exception("Ошибка при создании учетной записи администратора.");
                }
            }
        }
    }
}
