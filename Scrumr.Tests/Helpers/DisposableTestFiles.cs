using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Tests
{
    class DisposableTestFiles : IDisposable
    {
        public List<string> _filePaths = new List<string>();

        public string Create()
        {
            var directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "scrumr-test-databases"));
            
            if (!directory.Exists)
                directory.Create();

            var filePath = Path.Combine(directory.FullName, Path.GetRandomFileName());

            _filePaths.Add(filePath);
            return filePath;
        }

        public void Dispose()
        {
            _filePaths.ForEach(x => File.Delete(x));
        }
    }
}
