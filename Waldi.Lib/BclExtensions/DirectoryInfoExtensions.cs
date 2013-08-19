using System;
using System.IO;
using System.Linq;

namespace Waldi.BclExtensions
{
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Determines if the directory is writeable by the current user.
        /// </summary>
        /// <returns><c>true</c> if directory is writeable; otherwise, <c>false</c>.</returns>
        /// <param name="dir">Directory.</param>
        public static bool IsWriteable(this DirectoryInfo dir)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                // System.Security.AccessControl.DirectorySecurity ds = dir.GetAccessControl();
                dir.GetAccessControl();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the directory is writeable by the current user.
        /// </summary>
        /// <returns><c>true</c> if directory is writeable; otherwise, <c>false</c>.</returns>
        /// <param name="path">Full Path to Directory.</param>
        public static bool IsWriteable(string path)
        {
            return IsWriteable(new DirectoryInfo(path));
        }

		public static bool IsEmpty(this DirectoryInfo dir)
		{
			return !Directory.EnumerateFileSystemEntries(dir.FullName).Any();
		}

        public static bool Equals(this DirectoryInfo dir, DirectoryInfo other)
        {
            if (dir.FullName == other.FullName)
            {
                return true;
            }
            if (dir.FullName == null)
            {
                return false;
            }
            if (other.FullName == null)
            {
                return false;
            }
            return string.Equals(dir.FullName.Trim(Path.DirectorySeparatorChar), other.FullName.Trim(Path.DirectorySeparatorChar));
        }

		public static void CopyTo(this DirectoryInfo dir, string destDirName, bool recursive = false)
		{
			// Get the subdirectories for the specified directory.
			//DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ dir.FullName);
			}

			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (recursive)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryInfoExtensions.CopyTo(subdir, temppath, recursive);
				}
			}
		}
    }
}

