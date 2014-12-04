using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Tests
{
    class DisposableTestDatabase : IDisposable
    {
        public ScrumrContext Context { get; private set; }

        private string _testDbPath;

        public DisposableTestDatabase(DisposableTestWorkspace testFiles)
        {
            _testDbPath = testFiles.Create();
            FileSystem.Create(_testDbPath);
            Context = new ScrumrContext(_testDbPath);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
