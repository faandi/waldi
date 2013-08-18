using System;
using System.IO;

namespace Waldi.Lib
{
    public class DirectoryNotEmptyException : IOException
    {
        public DirectoryInfo Directory { get; private set;}

        public DirectoryNotEmptyException() : base()
        {
        }

        public DirectoryNotEmptyException(string message) : base(message)
        {
        }

        public DirectoryNotEmptyException(string message, DirectoryInfo directory) : base(message)
        {
            this.Directory = directory;
        }
    }
}