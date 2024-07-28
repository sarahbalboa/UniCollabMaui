using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniCollabMaui.Models;

namespace UniCollabMaui.Service
{
    public static class DatabaseService
    {
        static SQLiteAsyncConnection db;

        static async Task Init()
        {
            if (db != null)
                return;

            // Get an absolute path to the database file
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<AppTask>();
            await db.CreateTableAsync<User>();
        }

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

        public static async Task<IEnumerable<User>> GetUsers()
        {
            await Init();
            return await db.Table<User>().ToListAsync();
        }

        public static async Task<AppTask> GetAppTaskById(int id)
        {
            await Init();
            return await db.FindAsync<AppTask>(id);
        }

        public static async Task<User> ValidateUser(string username, string password)
        {
            await Init();
            return await db.Table<User>().FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public static async Task AddUser(User user)
        {
            await Init();
            await db.InsertAsync(user);
        }

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
