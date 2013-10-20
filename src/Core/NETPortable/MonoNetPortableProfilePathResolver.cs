using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGet
{
    /// <summary>
    /// Finds the .NET portable root directory on Mac and Linux.
    /// 
    /// This directory is equivalent to the following folder on Windows:
    /// 
    /// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable
    /// 
    /// Resolution is based on XBuild rules for resolving assembly references
    /// https://github.com/mono/mono/blob/master/mcs/class/Microsoft.Build.Tasks/Microsoft.Build.Tasks/GetReferenceAssemblyPaths.cs
    /// 
    /// The following directories are checked in the following order.
    /// 
    /// 1. Paths defined in the environment variable $XBUILD_FRAMEWORK_FOLDERS_PATH
    ///    with the .NETPortable directory appended.
    /// 2. /Library/Frameworks/Mono.framework/External/xbuild-frameworks/.NETPortable on Mac only
    /// 3. $prefix/lib/mono/xbuild-frameworks/.NETPortable
    /// </summary>
    public class MonoNetPortableProfilePathResolver
    {
        static readonly string MacXBuildFrameworksPath =
            "/Library/Frameworks/Mono.framework/External/xbuild-frameworks/";

        public string GetRootDirectory()
        {
            string[] paths = GetPossibleMonoNetPortablePaths();

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }

            return String.Empty;
        }

        string[] GetPossibleMonoNetPortablePaths()
        {
            string[] paths = GetPossibleXBuildFrameworkPaths();

            return paths
                .Select(frameworkPath => Path.Combine(frameworkPath, ".NETPortable"))
                .ToArray();
        }

        string[] GetPossibleXBuildFrameworkPaths()
        {
            List<string> paths = GetXBuildFrameworkFoldersPathEnvironmentVariable().ToList();

            if (EnvironmentUtility.IsMac)
            {
                paths.Add(MacXBuildFrameworksPath);
            }

            paths.Add(GetDefaultFrameworkPath());

            return paths.ToArray();
        }

        string[] GetXBuildFrameworkFoldersPathEnvironmentVariable()
        {
            string paths = Environment.GetEnvironmentVariable("XBUILD_FRAMEWORK_FOLDERS_PATH");

            if (paths == null)
            {
                return new string[0];
            }

            return paths.Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        string GetDefaultFrameworkPath()
        {
            string assemblyDirectory = Path.GetDirectoryName(typeof(Object).Assembly.Location);
            return Path.Combine(assemblyDirectory, "..", "xbuild-frameworks");
        }
    }
}
