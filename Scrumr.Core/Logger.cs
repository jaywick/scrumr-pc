using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Core
{
    public static class Logger
    {
        private static string LogFilePath;

        static Logger()
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                         "Jay Wick Labs",
                                         "Scrumr");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            LogFilePath = Path.Combine(directory, "scrumr.log");

            LogRaw("");
            LogRaw("------------------------------------------------------");
        }


        public static void Log(string message)
        {
             LogRaw(String.Format("[{0}]: {1}", DateTime.Now, message));
        }

        private static void LogRaw(string message)
        {
            using (var writer = File.AppendText(LogFilePath))
            {
                writer.WriteLine(String.Format("[{0}]: {1}", DateTime.Now, message));
            }
        }
    }
}
