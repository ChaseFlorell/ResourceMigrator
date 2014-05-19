﻿using System.IO;

// originally taken from http://stackoverflow.com/a/16987412/124069

namespace ResourceMigrator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sourceDirString = args[0];
            var androidTargetDir = args[1];
            var touchTargetDir = args[2];
            var touchNameSpace = args.Length >= 3 ? args[3] : "FutureState.AppCore.Touch";
            
            var sourceDir = new DirectoryInfo(sourceDirString);

            foreach (var sourceFile in sourceDir.GetFiles("*.resx"))
            {
                var resources = sourceFile.LoadResources();
                Droid.WriteToTarget(sourceFile, androidTargetDir, resources);
                Touch.WriteToTarget(sourceFile, touchTargetDir, resources, touchNameSpace);
            }
        }
    }
}