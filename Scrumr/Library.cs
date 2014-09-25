using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Scrumr
{
    class Library
    {
        public static List<T> Load<T>() where T : Entity
        {
            var folder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jay Wick Labs", "Scrumr", "data"));
            
            if (!folder.Exists)
                folder.Create();

            var file = new FileInfo(Path.Combine(folder.FullName, typeof(T).Name + "s.json"));

            if (!file.Exists)
                return new List<T>();

            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(file.FullName));
        }
    }
}
