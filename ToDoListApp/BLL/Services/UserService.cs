using Microsoft.AspNetCore.Identity;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enum;

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

        public async Task<(int, string)> CreateRole(UserRoles roles)
        {
            // Проверка, существует ли роль с таким именем
            var existingRole = await roleManager.FindByNameAsync(roles.Name);
            if (existingRole != null)
            {
                return (0, "Роль уже существует");
            }

            // Создание роли в базе данных
            var result = await roleManager.CreateAsync(new UserRoles { Name = roles.Name });

            if (result.Succeeded)
            {
                return (1, "Роль успешно создана");
            }

            return (1 , string.Join(", ", result.Errors));
        }

        public async Task<AddRolesRequestModel> AddRoleToUser(Guid id, Roles roles)
        {
            AddRolesRequestModel model = new ();
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                model.Id= id;
                model.Message = "Пользователь с ID не найден";
                model.Status = 0;

                return model;               
            }

            var roleExists = await roleManager.RoleExistsAsync(roles.ToString());

            if (!roleExists)
            {
                model.Role = roles.ToString();
                model.Message = "Роль не существует";
                model.Status = 0;

                return model;
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Contains(roles.ToString()))
            {
                model.Role = roles.ToString();
                model.Message = "У пользователя уже имеется роль";
                model.Status = 0;

                return model;
            }

            var result = await userManager.AddToRoleAsync(user, roles.ToString());

            if (result.Succeeded)
            {
                model.Id = id;
                model.Role = roles.ToString();
                model.Message = $"Пользователю с ID {id} успешно назначена роль {roles}";
                model.Status = 1;

                return model;
            }

            return model;
        }
    }
}
