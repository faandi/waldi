using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldi.Engine;

namespace Waldi.Packages
{
    public class PackageVersion : INamedItem, IComparable, IComparable<PackageVersion>, IEquatable<PackageVersion>
    {
		// TODO implement combined version name (ex. >1.1<2.2)

		/// <summary>
		/// Gets or sets the name (Version string).
		/// </summary>
		/// <value>Name of the version: [VersionQualifier][VersionNumber] or * for any Version or [VersionNumber] with default qualifier ('=').</value>
		public string Name { get; private set; }

        public VersionQualifier Qualifier {get; private set;}

		public string Number {get; private set;}

        public PackageVersion(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException ("name");
			}
            VersionQualifier qualifier = VersionQualifier.Equal;
            char qualifierchar = name[0];
            bool qualifierinname = this.TryParseVersionQualifier(qualifierchar, out qualifier);

            if (qualifier == VersionQualifier.Any && name.Length > 1)
            {
                throw new ArgumentException ("Name is not valid.","name");
            }
            if (qualifier != VersionQualifier.Any && name.Length < 2)
			{
				throw new ArgumentException ("Name is not valid.","name");
			}
            if (qualifierinname)
            {
                this.Number = name.Substring(1);
            }
            else
            {
                this.Number = name;
            }
            this.Qualifier = qualifier;
            this.Name = name;
		}

        private bool TryParseVersionQualifier(char qualifierchar, out VersionQualifier qualifier)
        {
            switch (qualifierchar)
            {
                case '*':
                    qualifier = VersionQualifier.Any;
                    return true;
                case '=':
                    qualifier = VersionQualifier.Equal;
                    return true;
                case '>':
                    qualifier = VersionQualifier.Greater;
                    return true;
                case '<':
                    qualifier = VersionQualifier.Lower;
                    return true;
                default:
                    qualifier = default(VersionQualifier);
                    return false;
            }
        }

        /// <summary>
        /// Returns if a version is valid for this version.
        /// </summary>
        /// <param name="other">Version to match.</param>
        public bool Matches(PackageVersion other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("version");
            }

            if (this.Qualifier == VersionQualifier.Any)
            {
                return true;
            }
            if (this.Qualifier == VersionQualifier.Equal)
            {
                return this.Number.Equals(other.Number);
            }
            if (this.Qualifier == VersionQualifier.Greater)
            {
                return other.Number.CompareTo(this.Number) > 0;
            }
            if (this.Qualifier == VersionQualifier.Lower)
            {
                return other.Number.CompareTo(this.Number) < 0;
            }
            throw new Exception("Unkown VersionQualifier.");
        }

        #region IComparable implementation

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException ("obj");
            }
            if (!(obj is PackageVersion))
            {
                throw new ArgumentException ("Obj is not of Type PackageVersion.","obj");
            }
            return this.CompareTo(obj as PackageVersion);
        }

        #endregion

        #region IComparable implementation

        public int CompareTo(PackageVersion other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }
            if (other.Qualifier == VersionQualifier.Any)
            {
                return 1;
            }
            return other.Number.CompareTo(this.Number);
        }

        #endregion

		#region IEquatable implementation

		public bool Equals (PackageVersion other)
		{
			return PackageVersion.Equals(this, other);
		}

		#endregion

		public static bool Equals(PackageVersion a, PackageVersion b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return string.Equals (a.Name, b.Name);
		}
    }
}
