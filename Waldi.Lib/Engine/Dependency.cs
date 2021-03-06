﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldi.Packages;

namespace Waldi.Engine
{
	public class Dependency : INamedItem, IEquatable<Dependency>
    {
		const char namesep = '.';

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>Name of the dependency: [PackageName].[FeatureName] or [PackageName] if the package has no Feature.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>Version string, prepend =,&lt;,&gt; if needed. Asterisk (*) is also supported.</value>
		public PackageVersion Version { get; set; }        
        
		public bool IsOptional { get; set; }

		public string PackageName {get; private set;}

		public string FeatureName {get; private set;}

		public Dependency(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException ("name");
			}
			string[] nameparts = name.Split (namesep);
			if (nameparts.Length > 2)
			{
				throw new ArgumentException ("Name is not valid.","name");
			}
			this.PackageName = nameparts [0];
			if (nameparts.Length > 1)
			{
				this.FeatureName = nameparts [1];
			}
			this.Name = name;
			this.Version = new PackageVersion("*");
			this.IsOptional = false;
		}

		public bool Equals (Dependency other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			bool areequal = string.Equals (this.Name, other.Name);
			areequal = areequal && this.IsOptional == other.IsOptional;
			areequal = areequal && PackageVersion.Equals (this.Version, other.Version);
			return areequal;
		}

		public static bool Equals (Dependency a, Dependency b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return a.Equals (b);
		}
    }
}
