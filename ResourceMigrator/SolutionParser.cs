using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ResourceMigrator
{
    // from http://stackoverflow.com/a/4634505/124069
    public class SolutionParser
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        private static readonly Type MsSolutionParser;
        private static readonly PropertyInfo SolutionParserSolutionReader;
        private static readonly MethodInfo SolutionParserParseSolution;
        private static readonly PropertyInfo SolutionParserProjects;

        static SolutionParser()
        {
            MsSolutionParser =
                Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                             false, false);
            if (MsSolutionParser == null) return;
            
            SolutionParserSolutionReader = MsSolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            SolutionParserProjects = MsSolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            SolutionParserParseSolution = MsSolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public SolutionParser(string solutionFileName)
        {
            if (MsSolutionParser == null)
            {
                throw new InvalidOperationException(
                    "Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }
            var solutionParser = MsSolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
            using (var streamReader = new StreamReader(solutionFileName))
            {
                SolutionParserSolutionReader.SetValue(solutionParser, streamReader, null);
                SolutionParserParseSolution.Invoke(solutionParser, null);
            }
            var array = (Array) SolutionParserProjects.GetValue(solutionParser, null);
            var projects = array.Cast<object>().Select((t, i) => new SolutionProject(array.GetValue(i))).ToList();
            Projects = projects;
        }

        public IList<SolutionProject> Projects { get; private set; }
    }

    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        private static readonly Type MsProjectInSolution;
        private static readonly PropertyInfo MsProjectName;
        private static readonly PropertyInfo MsRelativePath;
        private static readonly PropertyInfo MsProjectGuid;

        static SolutionProject()
        {
            MsProjectInSolution =
                Type.GetType(
                    "Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false,
                    false);
            if (MsProjectInSolution == null) return;
            
            MsProjectName = MsProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            MsRelativePath = MsProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
            MsProjectGuid = MsProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public SolutionProject(object solutionProject)
        {
            ProjectName = MsProjectName.GetValue(solutionProject, null) as string;
            RelativePath = MsRelativePath.GetValue(solutionProject, null) as string;
            ProjectGuid = MsProjectGuid.GetValue(solutionProject, null) as string;
        }

        public string ProjectName { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectGuid { get; private set; }
    }
}