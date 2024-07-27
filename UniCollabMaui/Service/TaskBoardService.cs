using SQLite;
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
            {
                return;
            }

            // Get an absolute path to the database file
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<AppTask>();
            //await db.CreateTableAsync<User>();
        }

        public static async Task AddAppTask(int taskId, string title, string description, string assignedToUser)
        {
            await Init();

            var appTask = new AppTask
            {
                Id = taskId,
                Title = title,
                Description = description,
                AssignedToUser = assignedToUser
            };

            var id = await db.InsertAsync(appTask);

        }
        public static async Task RemoveAppTask(int id)
        {
            await Init();
            await db.DeleteAsync<AppTask>(id);
        }
        public static async Task <IEnumerable<AppTask>>GetAppTask()
        {
            await Init();
            var myTask =  await db.Table<AppTask>().ToListAsync();
            return myTask;
        }


    }
}
