using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    public class Preferences
    {
        private const string AppKey = @"Software\Jay Wick Labs\Scrumr";

        public const string SourceFileKey = "Source File";
        public const string DefaultProjectKey = "Default Project";
        public const string ShowClosedTickets = "Show Closed Tickets";
        public const string ShowEmptyFeatures = "Show Empty Features";

        public string this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public string this[string key, string defaultValue]
        {
            get
            {
                if (String.IsNullOrWhiteSpace(key))
                    return defaultValue;

                return GetValue(key);
            }
        }

        private static string GetValue(string key)
        {
            var subKey = Registry.CurrentUser.OpenSubKey(AppKey, false);

            if (subKey == null)
                return null;

            return subKey.GetValue(key) as string;
        }

        public static void SetValue(string key, object value)
        {
            var subKey = Registry.CurrentUser.OpenSubKey(AppKey, true);

            if (subKey == null)
                subKey = Registry.CurrentUser.CreateSubKey(AppKey);

            subKey.SetValue(key, value.ToString());
        }
    }
}
