using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

// originally taken from http://stackoverflow.com/a/16987412/124069

namespace ResourceMigrator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var solutionPath = args[0];

            var solution = GetSolutionFromPath(solutionPath);
            var projects = GetProjects(solution, solutionPath);

            var touchTuple = projects.FirstOrDefault(p => p.Item2 == PlatformType.Touch);
            var droidTuple = projects.FirstOrDefault(p => p.Item2 == PlatformType.Droid);
            var pclTuple = projects.FirstOrDefault(p => p.Item2 == PlatformType.Pcl);

            if(pclTuple ==null) throw new Exception("Your resource files must be located in a PCL.");
            var resourceFiles = GetAllResourceFiles(pclTuple.Item3);
            
            foreach (var file in resourceFiles)
            {
                var fileInfo = new FileInfo(file);
                var resources = fileInfo.LoadResources();

                if (droidTuple != null)
                {
                    Droid.WriteToTarget(fileInfo, Path.Combine(droidTuple.Item3, "resources/values/"), resources);
                }

                if(touchTuple != null)
                {
                    Touch.WriteToTarget(fileInfo, Path.Combine(touchTuple.Item3, "resources/"), resources, touchTuple.Item1 + ".Resources");
                    
                }
            }
        }

        /// <summary>
        /// Returns Tuple / Namespace/ Type/ ProjectPath
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="solutionPath"></param>
        /// <returns></returns>
        private static IList<Tuple<string, PlatformType, string>> GetProjects(SolutionParser solution, string solutionPath)
        {
            var projects = new List<Tuple<string, PlatformType, string>>();

            foreach (var proj in solution.Projects)
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(Path.Combine(solutionPath, proj.RelativePath));

                var mgr = new XmlNamespaceManager(xmldoc.NameTable);
                mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

                var elemList = xmldoc.GetElementsByTagName("Import");
                for (var i = 0; i < elemList.Count; i++)
                {
                    var xmlAttributeCollection = elemList[i].Attributes;
                    if (xmlAttributeCollection == null) continue;

                    var attrVal = xmlAttributeCollection["Project"].Value;
                    var projectPath = Path.Combine(solutionPath, proj.RelativePath).Replace(proj.ProjectName + ".csproj", "");

                    if (attrVal.Contains("MonoTouch"))
                    {
                        projects.Add(new Tuple<string, PlatformType, string>(proj.ProjectName, PlatformType.Touch, projectPath));
                    }
                    else if (attrVal.Contains("Android"))
                    {
                        projects.Add(new Tuple<string, PlatformType, string>(proj.ProjectName, PlatformType.Droid, projectPath));
                    }
                    else if (attrVal.Contains("Portable"))
                    {
                        projects.Add(new Tuple<string, PlatformType, string>(proj.ProjectName, PlatformType.Pcl, projectPath));
                    }
                }
            }
            return projects;
        }

        private static IEnumerable<string> GetAllResourceFiles(string solutionPath)
        {
            return Directory.GetFiles(solutionPath, "*.resx", SearchOption.AllDirectories);
        }

        private static SolutionParser GetSolutionFromPath(string solutionPath)
        {
            var files = Directory.GetFiles(solutionPath, "*.sln");
            return new SolutionParser(files[0]);
        }
    }

    public enum PlatformType
    {
        Touch,
        Droid,
        Pcl
    }
}