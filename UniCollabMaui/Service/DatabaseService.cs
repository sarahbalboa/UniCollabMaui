using Supabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCollabMaui.Models;

namespace UniCollabMaui.Service
{
    /// <summary>
    /// DatabaseServic class.
    /// Used to interact with the database stored in Supabase.com. 
    /// </summary>
    public static class DatabaseService
    {
        private static Client _client;

        /// <summary>
        /// Ini()
        /// Intialise the database service to connect to the Supabase database service
        /// </summary>
        /// <returns></returns>
        static async Task Init()
        {
            if (_client != null)
                return;

            const string SUPABASE_URL = "https://hecqgqvmpkxkbkaayvns.supabase.co";
            const string SUPABASE_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImhlY3FncXZtcGt4a2JrYWF5dm5zIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTczMDQ2NDcxNywiZXhwIjoyMDQ2MDQwNzE3fQ.SKaFmtfVUgRHo5tqtm4xLUJdp8IirdPdY4Mq6pHDVtU";

            _client = new Client(SUPABASE_URL, SUPABASE_KEY);
            await _client.InitializeAsync();

        }

        //----------------------   User methods  -------------------------
        /// <summary>
        /// AddUser()
        /// Add a user to the database asynchroniously.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="active"></param>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task AddUser(string name, bool active, int username, string email, string password, int role)
        {
            await Init();
            var user = new User { Name = name, Active = active, Username = username, Email = email, Password = password,  RoleId = role,};

            // Insert the user record into the Supabase User table
            var response = await _client.From<User>().Insert(user);

        }

        /// <summary>
        /// UpdateUser()
        /// Update an existing user record asynchroniously from teh User table on the database.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="active"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task UpdateUser(int userId, string name, bool active, int role)
        {
            await Init();
            var user = await _client.From<User>().Filter("Id", Postgrest.Constants.Operator.Equals, userId).Single();
            if (user != null)
            {
                user.Name = name;
                user.Active = active;
                user.RoleId = role;
                await _client.From<User>().Update(user);
            }
        }

        /// <summary>
        /// ValidateUser()
        /// Validate the password and username from a matching user record on the User table database asynchroniously.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<User> ValidateUser(string username, string password)
        {
            await Init();
            var user = await _client.From<User>()
                .Filter("Username", Postgrest.Constants.Operator.Equals, username)
                .Filter("Password", Postgrest.Constants.Operator.Equals, password)
                .Single();
            return user;
        }

        /// <summary>
        /// GetUserbyId()
        /// Get a user from the User table using the ID attribute asynchroniously.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<User> GetUserById(int userId)
        {
            await Init();
            return await _client.From<User>()
                .Filter("Id", Postgrest.Constants.Operator.Equals, userId)
                .Single();
        }

        /// <summary>
        ///  GetUsers()
        ///  Get all Users from the User table asynchroniously.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<User>> GetUsers()
        {
            await Init();
            var users = await _client.From<User>().Get();
            return users.Models; // Get the list of user models
        }

        /// <summary>
        /// ValidateUniqueUser()
        /// Validate that there does not exist an existing user with the same username on the User table.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateUniqueUser(int username)
        {
            await Init();
            var user = await _client.From<User>().Filter("Username", Postgrest.Constants.Operator.Equals, username).Single();
            return user == null;
        }

        //----------------- Session methods --------------------------
        /// <summary>
        /// CreateSession()
        /// Create a session record for the user in teh Session table using the user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<string> CreateSession(int userId)
        {
            await Init();
            var sessionId = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(1); // 1-hour session expiry
            var session = new Session
            {
                SessionId = sessionId,
                UserId = userId,
                ExpiresAt = expiresAt
            };
            await _client.From<Session>().Insert(session);
            return sessionId;
        }

        /// <summary>
        /// GetUserIdFromSession()
        /// Get the user id from a sesson id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<int?> GetUserIdFromSession(string sessionId)
        {
            await Init();
            var session = await _client.From<Session>()
                .Filter("SessionId", Postgrest.Constants.Operator.Equals, sessionId)
                .Filter("ExpiresAt", Postgrest.Constants.Operator.GreaterThan, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .Single();
            return session?.UserId;
        }

        /// <summary>
        /// Logout()
        /// Delete the session from the user using the session Id when logging out/
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task Logout(string sessionId)
        {
            await Init();
            var session = await _client.From<Session>()
                .Filter("SessionId", Postgrest.Constants.Operator.Equals, sessionId)
                .Single();
            if (session != null)
            {
                await _client.From<Session>().Delete(session);
            }
        }

        //----------------- Role-based access control methods --------------------
        /// <summary>
        /// GetUserRole()
        /// Get the user roe using the user Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<Role> GetUserRole(int userId)
        {
            await Init();
            var user = await _client.From<User>().Filter("Id", Postgrest.Constants.Operator.Equals, userId).Single();
            if (user == null) return null;
            return await _client.From<Role>().Filter("Id", Postgrest.Constants.Operator.Equals, user.RoleId).Single();
        }

        //----------------- Task methods --------------------------
        /// <summary>
        /// AddAppTask()
        /// Add a new Task record to the AppTask table asynchroniously.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="column"></param>
        /// <param name="priority"></param>
        /// <param name="assignedToUserId"></param>
        /// <returns></returns>
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
            await _client.From<AppTask>().Insert(appTask);
        }

        /// <summary>
        /// GetAppTaskById()
        /// Get an Task from the Task table using its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<AppTask> GetAppTaskById(int id)
        {
            await Init();

            // Fetch the AppTask by its ID
            return await _client.From<AppTask>()
                .Filter("Id", Postgrest.Constants.Operator.Equals, id)
                .Single();

        }

        /// <summary>
        /// UpdateAppTask()
        /// Update a Task record values from the Task table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="column"></param>
        /// <param name="priority"></param>
        /// <param name="assignedToUserId"></param>
        /// <returns></returns>
        public static async Task UpdateAppTask(int id, string title, string description, string column, string priority, int assignedToUserId)
        {
            await Init();
            var task = await _client.From<AppTask>().Filter("Id", Postgrest.Constants.Operator.Equals, id).Single();
            if (task != null)
            {
                task.Title = title;
                task.Description = description;
                task.Column = column;
                task.Priority = priority;
                task.AssignedToUserId = assignedToUserId;
                await _client.From<AppTask>().Update(task);
            }
        }

        /// <summary>
        /// RemoveAppTask()
        /// Remove a Task record from the AppTask table using the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task RemoveAppTask(int id)
        {
            await Init();
            var task = await _client.From<AppTask>().Filter("Id", Postgrest.Constants.Operator.Equals, id).Single();
            if (task != null)
            {
                await _client.From<AppTask>().Delete(task);
            }
        }

        /// <summary>
        /// Get all the Tasks records from the AppTask table.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<AppTask>> GetAppTasks()
        {
            await Init();
            var tasks = await _client.From<AppTask>().Get();
            return tasks.Models;
        }

        //-------------------  Role methods   ------------------
        /// <summary>
        /// AddRole()
        /// Add a role to the Role table using a role object
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task AddRole(Role role)
        {
            await Init();
            var existingRole = await _client.From<Role>().Filter("RoleName", Postgrest.Constants.Operator.Equals, role.RoleName).Single();
            if (existingRole == null)
            {
                await _client.From<Role>().Insert(role);
            }
        }

        /// <summary>
        /// Get all the role records in the Role table.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<Role>> GetRoles()
        {
            await Init();
            var roles = await _client.From<Role>().Get();
            return roles.Models;
        }

        /// <summary>
        /// GetRoleById()
        /// Get a role record from the Role table using the role id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task<Role> GetRoleById(int roleId)
        {
            await Init();
            return await _client.From<Role>()
                .Filter("Id", Postgrest.Constants.Operator.Equals, roleId)
                .Single();
        }

        /// <summary>
        /// UpdateRole()
        /// Update a role record from the Role table.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="name"></param>
        /// <param name="active"></param>
        /// <param name="isRoleAdmin"></param>
        /// <param name="isTaskAdmin"></param>
        /// <param name="isTaskViewer"></param>
        /// <param name="isProgressViewer"></param>
        /// <returns></returns>
        public static async Task UpdateRole(int roleId, string name, bool active, bool isRoleAdmin, bool isTaskAdmin, bool isTaskViewer, bool isProgressViewer)
        {
            await Init();

            // Fetch the existing role
            var role = await _client.From<Role>()
                .Filter("Id", Postgrest.Constants.Operator.Equals, roleId)
                .Single();

            if (role != null)
            {
                // Modify the properties
                role.RoleName = name;
                role.Active = active;
                role.IsRoleAdmin = isRoleAdmin;
                role.IsTaskAdmin = isTaskAdmin;
                role.IsTaskViewer = isTaskViewer;
                role.IsProgressViewer = isProgressViewer;

                // Update the role
                var response = await _client.From<Role>().Update(role);
            }
        }
    }
}
