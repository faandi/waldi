using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waldi.Engine
{
    // TODO remove ?
	public class File : IEquatable<File>, INamedItem
	{
		private string name;
		public string Name
		{ 
			get
			{
				return this.name;
			}
			set
			{
				if (string.IsNullOrEmpty (value))
				{
					throw new ArgumentException ("Value cannot be null or empty.","value");
				}
				this.name = value;
			}
		}
		public string Path { get; set; }
		public FileType Type { get; set; }

		public bool Equals (File other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			bool areequal = string.Equals (this.Path, other.Path);
			areequal = areequal && Enum.Equals(this.Type, other.Type);
			return areequal;
		}
	}
}
