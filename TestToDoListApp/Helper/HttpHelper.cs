using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListAppTest.Helper
{
    internal class HttpHelper
    {
        public static StringContent GetJsonHttpContent(object items)
        {
            return new StringContent(JsonConvert.SerializeObject(items), Encoding.UTF8, "application/json");
        }

        internal static class Urls
        {
            public readonly static string Register = "/api/auth/registeration";
            public readonly static string Login = "/api/auth/login";
            public readonly static string AddRoles = "/api/admin/roles";
            public readonly static string GetAllTasks = "/api/admin/tasks";
            public readonly static string GetTaskById = "/api/admin/tasks";
            public readonly static string PutUserTask = "/api/admin/tasks";
            public readonly static string PostTask = "/api/admin/tasks";
            public readonly static string DeleteTask = "/api/admin/tasks";
            public readonly static string RefreshToken = "/api/token/refresh";
            public readonly static string Revoke = "/api/token/revoke";
            public readonly static string RevokeAll = "/api/token/revoke-all";
            public readonly static string GetAllTasksUser = "/api/user-tasks";
            public readonly static string GetUserTaskById = "/api/user-tasks";
            public readonly static string UpdateUserTask = "/api/user-tasks";
        }
    }
}
