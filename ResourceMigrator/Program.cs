using System;
using System.IO;
using System.Linq;

// originally taken from http://stackoverflow.com/a/16987412/124069

namespace ResourceMigrator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var solutionPath = args[0];

            var solution = FileHandler.GetSolutionFromPath(solutionPath);
            var projects = FileHandler.GetProjects(solution, solutionPath);

            var touchTuple = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Touch);
            var droidTuple = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Droid);
            var pclTuple = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Pcl);

            if (pclTuple == null) throw new Exception("Your resource files must be located in a PCL.");
            var resourceFiles = FileHandler.GetAllResourceFiles(pclTuple.ProjectPath);

            foreach (var file in resourceFiles)
            {
                var fileInfo = new FileInfo(file);
                var resources = fileInfo.LoadResources();

                // create the Android resources
                if (droidTuple != null)
                {
                    Droid.WriteToTarget(fileInfo, Path.Combine(droidTuple.ProjectPath, "resources/values/"), resources);
                }

                // create the iOS resources
                if (touchTuple != null)
                {
                    Touch.WriteToTarget(fileInfo, Path.Combine(touchTuple.ProjectPath, "resources/"), resources, touchTuple.ProjectNamespace + ".Resources");
                }
            }
        }
    }
}