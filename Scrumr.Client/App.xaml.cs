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
        public static Preferences Preferences { get; private set; }

        public static readonly int SchemaVersion = 1;

        public App()
        {
            Preferences = new Preferences();
        }
    }
}
