using Supabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCollabMaui.Models;

namespace UniCollabMaui.Service
{
    public static class DatabaseService
    {
        private static Client _client;

        // Initialize Supabase Client
        static async Task Init()
        {
            if (_client != null)
                return;

            const string SUPABASE_URL = "https://pqkgernhpuwfoviiqder.supabase.co";
            const string SUPABASE_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBxa2dlcm5ocHV3Zm92aWlxZGVyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzA2Mzg4MDksImV4cCI6MjA0NjIxNDgwOX0.qOTwLmG-uhGXxbbeGNpVWbJySSYwNLrjBr6gzBuAvfM";

            _client = new Client(SUPABASE_URL, SUPABASE_KEY);
            await _client.InitializeAsync();

        }

        //----------------------   User methods  -------------------------

        public static async Task AddUser(string name, bool active, int username, string email, string password)
        {
            await Init();
            var user = new User
            {
                Name = name,
                Active = active,
                Username = username,
                Email = email,
                Password = password,
            };

            // Insert the user record into the Supabase User table
            var response = await _client.From<User>().Insert(user);

        }


        public static async Task UpdateUser(int userId, string name, bool active)
        {
            await Init();
            var user = await _client.From<User>().Filter("Id", Postgrest.Constants.Operator.Equals, userId).Single();
            if (user != null)
            {
                user.Name = name;
                user.Active = active;
                await _client.From<User>().Update(user);
            }
        }

        public static async Task<User> ValidateUser(string username, string password)
        {
            await Init();
            var user = await _client.From<User>()
                .Filter("Username", Postgrest.Constants.Operator.Equals, username)
                .Filter("Password", Postgrest.Constants.Operator.Equals, password)
                .Single();
            return user;
        }
        public static async Task<User> GetUserById(int userId)
        {
            await Init();
            return await _client.From<User>()
                .Filter("Id", Postgrest.Constants.Operator.Equals, userId)
                .Single();
        }

        // Get all Users
        public static async Task<IEnumerable<User>> GetUsers()
        {
            await Init();
            var users = await _client.From<User>().Get();
            return users.Models; // Get the list of user models
        }

        public static async Task<bool> ValidateUniqueUser(int username)
        {
            await Init();
            var user = await _client.From<User>().Filter("Username", Postgrest.Constants.Operator.Equals, username).Single();
            return user == null;
        }



        //----------------- Session methods --------------------------

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

        public static async Task<int?> GetUserIdFromSession(string sessionId)
        {
            await Init();
            var session = await _client.From<Session>()
                .Filter("SessionId", Postgrest.Constants.Operator.Equals, sessionId)
                .Filter("ExpiresAt", Postgrest.Constants.Operator.GreaterThan, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .Single();
            return session?.UserId;
        }

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


        //----------------- Task methods --------------------------

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

        public static async Task<AppTask> GetAppTaskById(int id)
        {
            await Init();

            // Fetch the AppTask by its ID
            return await _client.From<AppTask>()
                .Filter("Id", Postgrest.Constants.Operator.Equals, id)
                .Single();

        }

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

        public static async Task RemoveAppTask(int id)
        {
            await Init();
            var task = await _client.From<AppTask>().Filter("Id", Postgrest.Constants.Operator.Equals, id).Single();
            if (task != null)
            {
                await _client.From<AppTask>().Delete(task);
            }
        }

        public static async Task<IEnumerable<AppTask>> GetAppTasks()
        {
            await Init();
            var tasks = await _client.From<AppTask>().Get();
            return tasks.Models;
        }

    }
}
