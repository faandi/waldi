using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/// <summary>
/// Logic for deploying items for tests.
/// </summary>
internal static class ItemDeployment
{
	/// <summary>
	/// Call in subclass to deploy items before testing.
	/// </summary>
	/// <param name="items">Items to deploy, relative to project root.</param>
	/// <param name="retainDirectories">Retain directory structure of source items?</param>
	/// <exception cref="FileNotFoundException">A source item was not found.</exception>
	/// <exception cref="DirectoryNotFoundException">The target deployment directory was not found</exception>
	public static void DeployFiles(IEnumerable<string> items, bool retainDirectories=false)
	{
		var environmentDir = new DirectoryInfo(Environment.CurrentDirectory);
		var binFolderPath = GetDeploymentDirectory();

		foreach (var item in items)
		{
			if (string.IsNullOrWhiteSpace(item))
			{
				continue;
			}

			string dirPath = retainDirectories ? Path.GetDirectoryName(item) : "";
			var filePath = item.Replace("/", Path.DirectorySeparatorChar.ToString());
			var itemPath = new Uri(Path.Combine(environmentDir.Parent.Parent.FullName,
			                                    filePath)).LocalPath;
			if (!File.Exists(itemPath))
			{
				throw new FileNotFoundException(string.Format("Can't find deployment source item '{0}'", itemPath));
			}

			if (!Directory.Exists(binFolderPath))
				throw new DirectoryNotFoundException(string.Format("Deployment target directory doesn't exist: '{0}'", binFolderPath));
			var dirPathInBin = Path.Combine(binFolderPath, dirPath);
			if (!Directory.Exists(dirPathInBin))
				Directory.CreateDirectory(dirPathInBin);
			var itemPathInBin = new Uri(Path.Combine(binFolderPath, dirPath, Path.GetFileName(filePath))).LocalPath;
			if (File.Exists(itemPathInBin))
			{
				File.Delete(itemPathInBin);
			}
			File.Copy(itemPath, itemPathInBin);
		}
	}

	/// <summary>
	/// Get directory test is deployed in.
	/// </summary>
	/// <returns></returns>
	public static string GetDeploymentDirectory()
	{
		return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
	}

}