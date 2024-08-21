using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UniCollabMaui.Service
{


    public class Logger
    {
        private static readonly string logFilePath ;

        static Logger()
        {
            // Get the path to the user's Downloads folder
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsDirectory = Path.Combine(downloadsPath, "Downloads");

            // Combine the Downloads directory path with the log file name
            logFilePath = Path.Combine(downloadsDirectory, "log.txt");
        }

        public static void Log(string message)
        {
            try
            {
                // Write the log message to the file
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during logging
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
