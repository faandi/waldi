using System;

namespace Waldi.Lib
{
    public class PackageIoException : Exception
    {
        public string Packagename { get; private set;}

        public PackageIoException(string message) : base(message)
        {
        }

        public PackageIoException(string message, string packagename) : base(message)
        {
            this.Packagename = packagename;
        }

        public PackageIoException(string message, string packagename, Exception innerException) : base(message, innerException)
        {
            this.Packagename = packagename;
        }
    }
}

