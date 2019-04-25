using Seron.Models;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Seron.Models
{
    public class Cell
    {
        public Board Board { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public List<CellHeight> CellsHeight { get; set; }

        public static int CUBEWIDTH = 64;
        public static int CUBEHEIGHT = 45;
        public static int WIDTHOFFSET = CUBEWIDTH / 2;
        public static int LENGTHOFFSET = CUBEWIDTH / 4;
        public static int HEIGHTOFFSET = CUBEHEIGHT - (LENGTHOFFSET * 2);

        public int AppearingStep { get; set; } = FRAMESTOAPPEAR;

        public static int FRAMESTOAPPEAR = 64;
        public static int APPEARINGFACTOR = 4;

        public Cell(Board board, string type, int x, int y)
        {
            this.Board = board;

            this.X = x;
            this.Y = y;

            this.CellsHeight = new List<CellHeight>();
            string[] typeheights = type.Split(",".ToCharArray());
            for (int i = 0; i < typeheights.Length; i++)
            {
                CellHeight heightadding = new CellHeight(this, typeheights[i]);
                heightadding.Position = new Vector2f((float)((x - y) * WIDTHOFFSET), (float)((x + y) * LENGTHOFFSET) - i * HEIGHTOFFSET + FRAMESTOAPPEAR * APPEARINGFACTOR - heightadding.HeightOffset);
                heightadding.FinalPosition = new Vector2f((float)((x - y) * WIDTHOFFSET), (float)((x + y) * LENGTHOFFSET) - i * HEIGHTOFFSET);
                this.CellsHeight.Add(heightadding);
            }
        }

        public void Draw(RenderWindow window, CellSelector selector = null)
        {
            this.AppearingStep--;
            foreach (CellHeight cellheight in this.CellsHeight)
            {
                if (this.AppearingStep >= 0)
                    cellheight.Position = new Vector2f(cellheight.Position.X, cellheight.Position.Y - APPEARINGFACTOR);

                window.Draw(cellheight);

                if (selector != null && cellheight == this.CellsHeight[0])
                    selector.Draw(window);
            }
        }
    }

    public class CellHeight : Sprite
    {
        public Cell Cell { get; set; }

        public Vector2f FinalPosition { get; set; }
        public int HeightOffset { get; set; } = 0;

        public CellTextureType TextureType { get; set; }
        public int TextureTypePosition { get; set; }

        public CellHeight(Cell cell, string type)
        {
            this.Cell = cell;

            this.TextureType = (CellTextureType)((int)Math.Pow(2, type.Substring(0, 1)[0] - 65));
            this.TextureTypePosition = int.Parse(type.Substring(1));

            if ((this.TextureType & (CellTextureType.Normal | CellTextureType.HeightSingle)) > 0)
                this.TextureRect = new IntRect(this.TextureTypePosition * Cell.CUBEWIDTH, 0, Cell.CUBEWIDTH, Cell.CUBEHEIGHT);
            else if (this.TextureType == CellTextureType.HeightDouble)
            {
                this.TextureRect = new IntRect(this.TextureTypePosition * Cell.CUBEWIDTH, Cell.CUBEHEIGHT, Cell.CUBEWIDTH, 2 * Cell.CUBEHEIGHT);
                this.HeightOffset = Cell.CUBEHEIGHT;
            }

            this.Texture = ((this.TextureType & CellTextureType.Normal) > 0) ? this.Cell.Board.World.Texture : this.Cell.Board.World.TextureHeight;
        }
    }

    public enum CellTextureType
    {
        Normal = 1,
        HeightSingle = 2,
        HeightDouble = 4
    }
}
