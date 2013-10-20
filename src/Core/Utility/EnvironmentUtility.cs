using System;
using System.Runtime.InteropServices;

namespace NuGet
{
    public static class EnvironmentUtility
    {
        private static bool _runningFromCommandLine;
        private static readonly bool _isMonoRuntime = Type.GetType("Mono.Runtime") != null;
        private static bool? _isMac;

        public static bool IsMonoRuntime
        {
            get
            {
                return _isMonoRuntime;
            }
        }

        public static bool RunningFromCommandLine
        {
            get
            {
                return _runningFromCommandLine;
            }
        }

        public static bool IsNet45Installed
        {
            get
            {
                using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(
                    Microsoft.Win32.RegistryHive.LocalMachine,
                    Microsoft.Win32.RegistryView.Registry32))
                {
                    using (var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
                    {
                        if (key == null)
                        {
                            return false;
                        }

                        object releaseKey = key.GetValue("Release");
                        return releaseKey is int && (int)releaseKey >= 378389;
                    }
                }
            }
        }

        // this will be called from nuget.exe
        public static void SetRunningFromCommandLine()
        {
            _runningFromCommandLine = true;
        }

        public static bool IsUnix
        {
            get
            {
                int platform = (int)Environment.OSVersion.Platform;
                return (platform == 4 || platform == 6 || platform == 128);
            }
        }

        public static bool IsMac
        {
            get
            {
                if (!_isMac.HasValue)
                {
                    _isMac = IsRunningOnMac();
                }
                return _isMac.Value;
            }
        }
        
        /// <summary>
        /// Taken from Pinta.
        /// 
        /// https://github.com/jpobst/Pinta/blob/master/Pinta.Core/Managers/SystemManager.cs
        /// 
        /// This code is based on code in Mono's Managed.Windows.Forms/XplatUI class.
        /// </summary>
        
        [DllImport("libc")]
        static extern int uname (IntPtr buf);

        static bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname()
                if (uname(buf) == 0)
                {
                    string os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (buf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
            return false;
        }
    }
}