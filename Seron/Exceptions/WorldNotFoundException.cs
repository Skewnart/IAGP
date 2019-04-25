using System;

namespace Seron.Exceptions
{
    class WorldNotFoundException : Exception
    {
        public WorldNotFoundException(string filename)
            : base($"Requested world {filename} does not exists.")
        {

        }
    }
}
