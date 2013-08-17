using System;
using System.IO;

namespace Waldi.BclExtensions
{
    public static class PathExtensions
    {
        /// <summary>
        /// Creates a uniquely named temporary directory on disk and returns if it was successfull.
        /// </summary>
        /// <returns><c>true</c>, if a temporary directory could be created, <c>false</c> otherwise.</returns>
        /// <param name="path">Full path of the temporary directory</param>
        public static bool GetTempPath(out string path)
        {
            int maxattempts = 10;
            int attempts = 0;
            DirectoryInfo tmpdir;
            do
            {
                tmpdir = new DirectoryInfo(Path.Combine(Path.GetTempPath(),Path.GetRandomFileName()));
                attempts++;
            }
            while(attempts <= maxattempts && tmpdir.Exists);
            if (attempts > maxattempts)
            {
				path = string.Empty;
                return false;
            }
            tmpdir.Create();
            path = tmpdir.FullName;
            return true;
        }
    }
}

