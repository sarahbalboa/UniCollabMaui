using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCollabMaui.Models;

namespace UniCollabMaui.Service
{
    public interface IDatabaseService
    {
        // User methods
        Task AddUser(string name, bool active, int username, string email, string password, int role);
        Task UpdateUser(int userId, string name, bool active, int role);
        Task<User> ValidateUser(string username, string password);
        Task<User> GetUserById(int userId);
        Task<IEnumerable<User>> GetUsers();
        Task<bool> ValidateUniqueUser(int username);

        // Session methods
        Task<string> CreateSession(int userId);
        Task<int?> GetUserIdFromSession(string sessionId);
        Task Logout(string sessionId);

        // Role-based access control methods
        Task<Role> GetUserRole(int userId);

        // Task methods
        Task AddAppTask(string title, string description, string column, string priority, int assignedToUserId);
        Task<AppTask> GetAppTaskById(int id);
        Task UpdateAppTask(int id, string title, string description, string column, string priority, int assignedToUserId);
        Task RemoveAppTask(int id);
        Task<IEnumerable<AppTask>> GetAppTasks();

        // Role methods
        Task AddRole(Role role);
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> GetRoleById(int roleId);
        Task UpdateRole(int roleId, string name, bool active, bool isRoleAdmin, bool isTaskAdmin, bool isTaskViewer, bool isProgressViewer);
    }
}
