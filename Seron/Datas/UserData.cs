using System;
using System.IO;

namespace Seron.Datas
{
    public class UserData
    {
        private static readonly string BASEFOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seron");
        public static readonly string WORLDFOLDER = Path.Combine(BASEFOLDER, "worlds");

        public static void Initialize()
        {
            if (!Directory.Exists(BASEFOLDER)) Directory.CreateDirectory(BASEFOLDER);
            if (!Directory.Exists(WORLDFOLDER)) Directory.CreateDirectory(WORLDFOLDER);
        }
    }
}
