using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UniCollabMaui.Models;

namespace UniCollabMaui.Service
{
    public static class DatabaseService
    {
        static SQLiteAsyncConnection db;
       
        static async Task AddDefaultRoles(SQLiteAsyncConnection db)
        {
            // Check if roles already exist
            var existingRoles = await db.Table<Role>().ToListAsync();
            if (existingRoles.Count > 0)
            {
                return; // Roles already exist, so exit the function
            }

            var roles = new List<Role>
            {
                new Role { RoleName = "Task Admin", Active = true, IsSystemRole = true, IsRoleAdmin = false, IsTaskAdmin = true, IsTaskViewer = true, IsProgressViewer = true},
                new Role { RoleName = "Administrator", Active = true, IsSystemRole = true, IsRoleAdmin = true, IsTaskAdmin = true, IsTaskViewer = true, IsProgressViewer = true},
                new Role { RoleName = "User", Active = true, IsSystemRole = true, IsRoleAdmin = false, IsTaskAdmin = false, IsTaskViewer = true, IsProgressViewer = true},
                new Role { RoleName = "Role Administrator", Active = true, IsSystemRole = true, IsRoleAdmin = true, IsTaskAdmin = false, IsTaskViewer = false, IsProgressViewer = false}
            };

            await db.InsertAllAsync(roles);
        }
        static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<AppTask>();
            await db.CreateTableAsync<User>();
            await db.CreateTableAsync<Role>();
            await db.CreateTableAsync<Session>();

            //----------------- Add default app roles ------------------

            await AddDefaultRoles(db);


        }

        //----------------------   User methods (unchanged) -------------------------

        public static async Task AddUser(string name, bool active, string username, string email, string password, int role)
        {
            await Init();
            var user = new User
            {
                Name = name,
                Active = active,
                Username = username,
                Email = email,
                Password = password,
                RoleId = role,
            };
            await db.InsertAsync(user);
        }
        public static async Task UpdateUser(int userId, string name, bool active, int role)
        {
            await Init();
            var user = await db.FindAsync<User>(userId);
            if (user != null)
            {
                user.Name = name;
                user.Active = active;
                user.RoleId = role;
                await db.UpdateAsync(user);
            }
        }
        public static async Task<User> ValidateUser(string username, string password)
        {
            await Init();
            return await db.Table<User>().FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public static async Task<bool> ValidateUniqueUser(string username)
        {
            await Init();
            var existingUser = await db.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            return existingUser == null;
        }

        // New methods for session management
        public static async Task<string> CreateSession(int userId)
        {
            await Init();
            var sessionId = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(1); // 1 hour session expiry
            var session = new Session
            {
                SessionId = sessionId,
                UserId = userId,
                ExpiresAt = expiresAt
            };
            await db.InsertAsync(session);
            return sessionId;
        }

        public static async Task<int?> GetUserIdFromSession(string sessionId)
        {
            await Init();
            var session = await db.Table<Session>().FirstOrDefaultAsync(s => s.SessionId == sessionId && s.ExpiresAt > DateTime.UtcNow);
            return session?.UserId;
        }
        public static async Task Logout(string sessionId)
        {
            await Init();
            var session = await db.Table<Session>().FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session != null)
            {
                await db.DeleteAsync(session);
            }
        }


        public static async Task<User> GetUserById(int userId)
        {
            await Init();
            return await db.FindAsync<User>(userId);
        }

        public static async Task<IEnumerable<User>> GetUsers()
        {
            await Init();
            return await db.Table<User>().ToListAsync();
        }
        // Role-based access control methods
        public static async Task<Role> GetUserRole(int userId)
        {
            await Init();
            var user = await db.FindAsync<User>(userId);
            var userRole = await db.FindAsync<Role>(user?.RoleId);
            return userRole;
        }

        //-----------------  Task methods --------------------------

        public static async Task AddAppTask(string title, string description, string column, string priority, int assignedToUserId)
        {
            await Init();
            var appTask = new AppTask
            {
                Title = title,
                Description = description,
                Column = column,
                Priority = priority,
                AssignedToUserId = assignedToUserId
            };
            await db.InsertAsync(appTask);
        }

        public static async Task UpdateAppTask(int id, string title, string description, string column, string priority, int assignedToUserId)
        {
            await Init();
            var appTask = await db.FindAsync<AppTask>(id);
            if (appTask != null)
            {
                appTask.Title = title;
                appTask.Description = description;
                appTask.Column = column;
                appTask.Priority = priority;
                appTask.AssignedToUserId = assignedToUserId;
                await db.UpdateAsync(appTask);
            }
        }

        public static async Task RemoveAppTask(int id)
        {
            await Init();
            await db.DeleteAsync<AppTask>(id);
        }

        public static async Task<IEnumerable<AppTask>> GetAppTasks()
        {
            await Init();
            return await db.Table<AppTask>().ToListAsync();
        }

        

        public static async Task<AppTask> GetAppTaskById(int id)
        {
            await Init();
            return await db.FindAsync<AppTask>(id);
        }

        //-------------------  Role methods   ------------------

        public static async Task AddRole(Role role)
        {
            await Init();
            var existingRole = await db.Table<Role>().FirstOrDefaultAsync(r => r.RoleName == role.RoleName);
            if (existingRole == null)
            {
                await db.InsertAsync(role);
            }
        }

        public static async Task<IEnumerable<Role>> GetRoles()
        {
            await Init();
            return await db.Table<Role>().ToListAsync();
        }
        public static async Task UpdateRole(int roleId, string name, bool active, bool isRoleAdmin, bool IsTaskAdmin, bool isTaskViewer, bool isProgressViewer)
        {
            await Init();
            var role = await db.FindAsync<Role>(roleId);
            if (role != null)
            {
                role.RoleName = name;
                role.Active = active;
                role.IsRoleAdmin = isRoleAdmin;
                role.IsTaskAdmin = IsTaskAdmin;
                role.IsTaskViewer = isTaskViewer;
                role.IsProgressViewer = isProgressViewer;
                await db.UpdateAsync(role);
            }
        }
        public static async Task<Role> GetRoleById(int roleId)
        {
            await Init();
            return await db.FindAsync<Role>(roleId);
        }
        // -----------------------------  Erase db data mothods ----------------------------
        public static async Task EraseAllUsersData()
        {
            await Init();
            await db.DeleteAllAsync<User>();
        }

        public static async Task EraseAllTasksData()
        {
            await Init();
            await db.DeleteAllAsync<AppTask>();
        }
    }
}
