﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace PathLengthChecker
{
	/// <summary>
	/// The type of Paths that should be included.
	/// </summary>
	[Flags]
	public enum FileSystemTypes
	{
		Files = 1,
		Directories = 2,
		All = Files | Directories
	}

	/// <summary>
	/// Options used when retrieving paths.
	/// </summary>
	public class PathSearchOptions
	{
		/// <summary>
		/// The root directory to search in.
		/// </summary>
		public string RootDirectory = string.Empty;

		/// <summary>
		/// The search pattern to match against.
		/// </summary>
		public string SearchPattern = "*";

		/// <summary>
		/// Specifies if we should search subdirectories or not.
		/// </summary>
		public SearchOption SearchOption = SearchOption.AllDirectories;

		/// <summary>
		/// Specifies if we should look for Files, Directories, or both.
		/// </summary>
		public FileSystemTypes TypesToGet = FileSystemTypes.All;

		/// <summary>
		/// The directory that the root directory should be replaced with in the found paths.
		/// Specify null to leave the original paths unmodified.
		/// </summary>
		public string RootDirectoryReplacement = null;
	}

	/// <summary>
	/// Class used to retrieve file system objects in a given path.
	/// </summary>
	public static class PathRetriever
	{
		/// <summary>
		/// Gets the paths.
		/// </summary>
		/// <param name="searchOptions">The search options to use.</param>
		public static IEnumerable<string> GetPaths(PathSearchOptions searchOptions)
		{
			// If no Search Pattern was provided, then find everything.
			if (string.IsNullOrEmpty(searchOptions.SearchPattern))
				searchOptions.SearchPattern = "*";

			// Get the paths according to the search parameters
			var paths = new List<string>();

			switch (searchOptions.TypesToGet)
			{
				case FileSystemTypes.All:
					paths = Directory.GetFileSystemEntries(searchOptions.RootDirectory, searchOptions.SearchPattern, searchOptions.SearchOption).ToList();
					break;
					
				case FileSystemTypes.Directories:
					paths = Directory.GetDirectories(searchOptions.RootDirectory, searchOptions.SearchPattern, searchOptions.SearchOption).ToList();
					break;

				case FileSystemTypes.Files:
					paths = Directory.GetFiles(searchOptions.RootDirectory, searchOptions.SearchPattern, searchOptions.SearchOption).ToList();
					break;
			}
			
			// Return each of the paths, replacing the Root Directory if specified to do so.
			foreach (var path in paths)
			{
				if (searchOptions.RootDirectoryReplacement == null)
					yield return path;
				else
					yield return path.Replace(searchOptions.RootDirectory, searchOptions.RootDirectoryReplacement);
			}
		}
	}
}
