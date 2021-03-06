﻿using System;
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

            var pcl = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Pcl);
            if (pcl == null) throw new Exception("Your resource files must be located in a PCL.");

            var phone = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Phone);
            var touch = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Touch);
            var droid = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Droid);

            var resourceFiles = FileHandler.GetAllResourceFiles(pcl.ProjectPath);

            foreach (var file in resourceFiles)
            {
                var fileInfo = new FileInfo(file);
                var resources = fileInfo.LoadResources();

                // create the Android resources
                if (droid != null)
                {
                    new Droid().WriteToTarget(droid, resources, fileInfo);
                }

                // create the iOS resources
                if (touch != null)
                {
                    new Touch().WriteToTarget(touch, resources, fileInfo);
                }

                // create the Windows Phone resources
                if (phone != null)
                {
                    // new Phone().WriteToTarget(phone, resources, fileInfo);
                }
            }
        }
    }
}