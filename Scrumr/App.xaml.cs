using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Scrumr
{
    public partial class App : Application
    {
        public static bool Overwrite { get; private set; }

        public App()
        {
            CheckDirectives();
        }

        private static void CheckDirectives()
        {
#if OVERWRITE
            Overwrite = true;
#else
            Overwrite = false;
#endif
        }
    }
}
