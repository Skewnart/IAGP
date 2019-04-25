using System;
using System.IO;

namespace Seron.Datas
{
    public class ProgramData
    {
        private static readonly string BASEFOLDER = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string TILESETFOLDER = Path.Combine(BASEFOLDER, "tilesets");

        public static void Initialize()
        {
            if (!Directory.Exists(BASEFOLDER)) Directory.CreateDirectory(BASEFOLDER);
            if (!Directory.Exists(TILESETFOLDER)) Directory.CreateDirectory(TILESETFOLDER);
        }
    }
}
