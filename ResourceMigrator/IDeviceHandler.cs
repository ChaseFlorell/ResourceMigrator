using System.Collections.Generic;
using System.IO;

namespace ResourceMigrator
{
    public interface IDeviceHandler
    {
        void WriteToTarget(ProjectModel project, IDictionary<string, string> strings, FileInfo sourceFile);
    }
}