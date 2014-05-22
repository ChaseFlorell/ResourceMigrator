using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ResourceMigrator
{
    public static class FileHandler
    {
        public static IList<ProjectModel> GetProjects(SolutionParser solution, string solutionPath)
        {
            var projects = new List<ProjectModel>();

            foreach (var proj in solution.Projects)
            {
                var xmldoc = new XmlDocument();
                try
                {
                    xmldoc.Load(Path.Combine(solutionPath, proj.RelativePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

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
                        projects.Add(new ProjectModel { ProjectNamespace = proj.ProjectName, ProjectPath = projectPath, PlatformType = PlatformType.Touch });
                    }
                    else if (attrVal.Contains("Android"))
                    {
                        projects.Add(new ProjectModel { ProjectNamespace = proj.ProjectName, ProjectPath = projectPath, PlatformType = PlatformType.Droid });
                    }
                    else if (attrVal.Contains("Portable"))
                    {
                        projects.Add(new ProjectModel { ProjectNamespace = proj.ProjectName, ProjectPath = projectPath, PlatformType = PlatformType.Pcl });
                    }
                }
            }
            return projects;
        }

        public static IEnumerable<string> GetAllResourceFiles(string solutionPath)
        {
            return Directory.GetFiles(solutionPath, "*.resx", SearchOption.AllDirectories);
        }

        public static SolutionParser GetSolutionFromPath(string solutionPath)
        {
            var files = Directory.GetFiles(solutionPath, "*.sln");
            return new SolutionParser(files[0]);
        }
    }
}