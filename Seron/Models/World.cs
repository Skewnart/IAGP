using Seron.Datas;
using Seron.Exceptions;
using SFML.Graphics;
using System.IO;

namespace Seron.Models
{
    public class World
    {
        public string Name { get; set; }
        public Board Board { get; set; }

        public Texture Texture { get; set; }
        public Texture TextureHeight { get; set; }

        /// <summary>
        /// Load a world from a file (in the "worlds" folder)
        /// </summary>
        /// <param name="filename">Name of the file (extension .swo)</param>
        public World(string filename, string texturename)
        {
            this.Texture = new Texture(Path.Combine(ProgramData.TILESETFOLDER, $"{texturename}.png"));
            this.TextureHeight = new Texture(Path.Combine(ProgramData.TILESETFOLDER, $"{texturename}heights.png"));

            this.Name = filename ?? "Nouveau";
            if (filename != null)
            {
                if (!filename.EndsWith(".swo")) filename += ".swo";
                filename = Path.Combine(UserData.WORLDFOLDER, filename);

                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);
                    this.Board = new Board(this, lines);
                }
                else
                    throw new WorldNotFoundException(filename);
            }
            else
            {
                this.Board = new Board(this);
            }
        }
    }
}
