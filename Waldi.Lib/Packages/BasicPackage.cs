using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Waldi.Engine;

namespace Waldi.Packages
{
	public class BasicPackage : IPackage, IEquatable<BasicPackage>
	{
		private string name;
		public string Name
		{ 
			get
			{
				return this.name;
			}
			private set
			{
				if (string.IsNullOrEmpty (value))
				{
					throw new ArgumentException ("Value cannot be null or empty.","value");
				}
				Regex regex = new Regex("^([0-9]|[A-Z]|[a-z]|\\.)+$");
				if (!regex.IsMatch (value))
				{
					throw new ArgumentException ("Invalid Feature/Package Name.","value");
				}
				this.name = value;
			}
		}

		public string Description { get; set; }
		public string Url { get; set; }
		public bool IsMaster { get; set; }

        public NamedItemList<PackageVersion> Versions { get; private set; }
		public DependencyList Dependencies { get; private set; }

		public PackageList Packages { get; private set; }
		public FeatureList Features { get; private set; }
		public PackageVersion SelectedVersion { get; set; }

		public BasicPackage(string name)
		{
			this.Name = name;
            this.Versions = new NamedItemList<PackageVersion>();
			this.Packages = new PackageList ();
			this.Features = new FeatureList ();
			this.Dependencies = new DependencyList();
            this.SelectedVersion = new PackageVersion("*");
		}

		public bool Equals (BasicPackage other)
		{
			return this.Equals ((IPackage)other);
		}

		public bool Equals (IPackage other)
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
			areequal = areequal && string.Equals (this.Description, other.Description);
			areequal = areequal && string.Equals (this.Url, other.Url);
			areequal = areequal && this.IsMaster == other.IsMaster;
			areequal = areequal && Enumerable.SequenceEqual<PackageVersion>(this.Versions, other.Versions);
			areequal = areequal && Enumerable.SequenceEqual<Dependency>(this.Dependencies, other.Dependencies);
			areequal = areequal && Enumerable.SequenceEqual<IPackage>(this.Packages, other.Packages);
			areequal = areequal && Enumerable.SequenceEqual<Feature>(this.Features, other.Features);
			areequal = areequal && PackageVersion.Equals (this.SelectedVersion, other.SelectedVersion);
			return areequal;
		}

		public static bool Equals (BasicPackage a, BasicPackage b)
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