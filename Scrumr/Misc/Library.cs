using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

namespace Scrumr
{
    class Library
    {
        public static Context Load()
        {
            return new Context
            {
                Projects = Library.Load<Project>(),
                Sprints = Library.Load<Sprint>(),
                Features = Library.Load<Feature>(),
                Tickets = Library.Load<Ticket>(),
            };
        }

        public static void Save(Context context)
        {
            Save<Project>(context.Projects);
            Save<Sprint>(context.Sprints);
            Save<Feature>(context.Features);
            Save<Ticket>(context.Tickets);
        }

        private static List<T> Load<T>() where T : Entity
        {
            var file = getFile<T>();

            if (!file.Exists)
                return new List<T>();

            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(file.FullName));
        }

        public static void Save<T>(List<T> entity) where T : Entity
        {
            var file = getFile<T>();
            File.WriteAllText(file.FullName, JsonConvert.SerializeObject(entity, Formatting.Indented));
        }

        private static DirectoryInfo getFolder()
        {
            var folder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jay Wick Labs", "Scrumr", "data"));

            if (!folder.Exists)
                folder.Create();

            return folder;
        }

        private static FileInfo getFile<T>() where T : Entity
        {
            return new FileInfo(Path.Combine(getFolder().FullName, typeof(T).Name + "s.json"));
        }
    }
}
