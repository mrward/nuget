
using System;
using System.IO;
using System.Globalization;
using System.Runtime.Versioning;
using EnvDTE;
using NuGet.Resources;
using NuGet.VisualStudio.Resources;

namespace NuGet.VisualStudio
{
    public class ScriptCsScriptExecutor
    {
        public void ExecuteInstallScript(string installPath, IPackage package, Project project, FrameworkName targetFramework, ILogger logger)
        {
            ExecuteScript(installPath, ScriptCsScripts.Install, package, project, targetFramework, logger);
        }

        public void ExecuteUninstallScript(string installPath, IPackage package, Project project, FrameworkName targetFramework, ILogger logger)
        {
            ExecuteScript(installPath, ScriptCsScripts.Uninstall, package, project, targetFramework, logger);
        }

        public void ExecuteInitScript(string installPath, IPackage package, ILogger logger)
        {
            ExecuteScript(installPath, ScriptCsScripts.Init, package, null, null, logger);
        }

        void ExecuteScript(string installPath, string scriptFileName, IPackage package, Project project, FrameworkName targetFramework, ILogger logger)
        {
            string fullPath;
            IPackageFile scriptFile;
            if (package.FindCompatibleToolFiles(scriptFileName, targetFramework, out scriptFile))
            {
                fullPath = Path.Combine(installPath, scriptFile.Path);
            }
            else
            {
                return;
            }

            if (File.Exists(fullPath))
            {
                if (project != null && scriptFile != null)
                {
                    // targetFramework can be null for unknown project types
                    string shortFramework = targetFramework == null ? string.Empty : VersionUtility.GetShortFrameworkName(targetFramework);

                    logger.Log(MessageLevel.Debug, NuGetResources.Debug_TargetFrameworkInfoPrefix, package.GetFullName(),
                        project.Name, shortFramework);

                    logger.Log(MessageLevel.Debug, NuGetResources.Debug_TargetFrameworkInfo_PowershellScripts,
                        Path.GetDirectoryName(scriptFile.Path), VersionUtility.GetTargetFrameworkLogString(scriptFile.TargetFramework));
                }

                string toolsPath = Path.GetDirectoryName(fullPath);
                string logMessage = String.Format(CultureInfo.CurrentCulture, VsResources.ExecutingScript, fullPath);

                // logging to both the Output window and progress window.
                logger.Log(MessageLevel.Info, logMessage);

                var script = new PackageScript(fullPath)
                {
                    RootPath = installPath,
                    ToolsPath = toolsPath,
                    Package = package,
                    Project = project
                };
                var session = new ScriptCsSession(logger);
                script.Run(session);
            }
        }
    }
}
