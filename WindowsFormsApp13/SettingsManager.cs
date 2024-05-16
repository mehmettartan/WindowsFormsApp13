using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp13
{
    public static class SettingsManager
    {
        public static string[] Settings { get; set; }

        public static void Initialize(string[] settings)
        {
            Settings = settings;
        }
    }    
}
