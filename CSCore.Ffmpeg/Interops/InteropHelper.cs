using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSCore.Ffmpeg.Interops
{
    internal static class InteropHelper
    {
        // ReSharper disable once InconsistentNaming
        public const string LD_LIBRARY_PATH = "LD_LIBRARY_PATH";

        public static void RegisterLibrariesSearchPath(string path)
        {
            if (!Directory.Exists(path))
            {
                var directory = FfmpegUtils.FindFfmpegDirectory(Environment.OSVersion.Platform);
                if (directory != null)
                    path = directory.FullName;
            }

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    SetDllDirectory(path);
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    string libPaths = Environment.GetEnvironmentVariable(LD_LIBRARY_PATH);
                    var searchPaths = libPaths?.Split(new [] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                    if (!searchPaths.Contains(path)) {
                        searchPaths.Add(path);
                        Environment.SetEnvironmentVariable(LD_LIBRARY_PATH, String.Join(Path.PathSeparator.ToString(), searchPaths));
                    }
                    break;
            }
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
    }
}
