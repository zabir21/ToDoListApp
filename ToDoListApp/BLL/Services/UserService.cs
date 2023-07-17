using Microsoft.AspNetCore.Identity;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enums;

namespace ToDoListApp.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<UserRoles> roleManager;
        public UserService(UserManager<User> userManager, RoleManager<UserRoles> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task AddRoleToUser(Guid userId, Roles role)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var roleExists = await roleManager.RoleExistsAsync(role.ToString());

            if (!roleExists)
            {
                throw new RoleNotExistException();
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Contains(role.ToString()))
            {
                return;
            }

            var result = await userManager.AddToRoleAsync(user, role.ToString());

            if (!result.Succeeded)
            {
                throw new RoleNotAddedToUserException($"Can't add role: {role} to user: {userId}");
            }
        }
    }
}
