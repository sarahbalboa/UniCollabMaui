using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniCollabMaui.Models;

namespace UniCollabMaui.Service
{
    public static class TaskBoardService
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

        public static async Task AddAppTask(string title, string description, string column, int assignedToUserId)
        {
            await Init();
            var appTask = new AppTask
            {
                Title = title,
                Description = description,
                Column = column,
                AssignedToUserId = assignedToUserId
            };
            await db.InsertAsync(appTask);
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
    }
}
