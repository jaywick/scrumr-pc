using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class FileSystem
    {
        public const string DefaultDatabase = "scrumr.sqlite";

        public static ScrumrContext LoadContext(string filename = DefaultDatabase)
        {
            if (File.Exists(DefaultDatabase))
                throw new FileNotFoundException("Database does not exist", filename);

            return new ScrumrContext(DefaultDatabase);
        }
    }
}
