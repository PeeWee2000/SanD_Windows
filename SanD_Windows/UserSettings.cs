using System.Configuration;

namespace SanD_Windows
{
    class UserSettings
    {
        public static string WhiteListLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["WhiteListLocation"];
            }
            set { }
        }
    }
}
