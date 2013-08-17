using System;
using System.IO;

namespace Waldi.BclExtensions
{
	public static class UriExtensions
    {
		/// <summary>
		/// Gets the URI for a file.
		/// </summary>
		/// <returns>The URI.</returns>
		/// <param name="fileuri">Absolute or relative URI string to a file. Relative URIs have to start with file://~/.</param>
		/// <param name="workinguri">Abosulte URI string to the directory which the fileuri is relative or null if fileuri is absolute.</param>
		public static Uri GetFileUri(string fileuri, string workinguri = null)
        {
			if (string.IsNullOrEmpty (workinguri) || !fileuri.StartsWith("file://~/"))
			{
				return new Uri (fileuri);
			}
			Uri wuri = new Uri (workinguri);
			return new Uri (wuri.AbsoluteUri + "/" + fileuri.Substring ("file://~/".Length));
        }
    }
}

