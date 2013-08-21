using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Waldi.Engine;

namespace Waldi.Packages
{
	public class Feature : IEquatable<Feature>, INamedItem
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
		public bool Enabled { get; set; }

		public Feature(string name)
		{
			this.Name = name;
		}

		public bool Equals (Feature other)
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
			areequal = areequal && this.Enabled == other.Enabled;
			return areequal;
		}

		public static bool Equals (Feature a, Feature b)
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
