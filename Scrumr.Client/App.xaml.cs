using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Scrumr.Client
{
    public partial class App : Application
    {
        public static bool Overwrite { get; private set; }

        public static Preferences Preferences { get; private set; }

        public App()
        {
            CheckDirectives();
            Preferences = new Preferences();
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
