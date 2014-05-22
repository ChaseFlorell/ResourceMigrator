using System.Reflection;

namespace ResourceMigrator
{
    public class Helpers
    {
        public static string GetAssemblyVersion()
        {
            return Assembly.GetAssembly(typeof (Program)).GetName().Version.ToString();
        }
    }
}