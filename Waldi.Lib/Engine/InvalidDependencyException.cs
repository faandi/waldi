using System;
using Waldi.Packages;


namespace Waldi.Engine
{
	public class InvalidDependencyException : Exception
	{
		public IPackage Package { get; set; }
		public IPackage Dependency { get; set; }

		public InvalidDependencyException(string message) : base(message)
		{

		}

        public InvalidDependencyException(string message, Exception innerException, IPackage package = null) : base(message, innerException)
        {
            this.Package = package;
        }

		public InvalidDependencyException(string message, IPackage package, IPackage dependency) : base(message)
		{
			this.Package = package;
			this.Dependency = dependency;
		}
	}
}

