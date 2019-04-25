using Seron.Extensions;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seron.Models
{
    public class Board
    {
        public World World { get; set; }
        public Cell[][] Cells { get; set; }

        public IntPoint Size => this.Cells != null ? new IntPoint(this.Cells.Length, this.Cells[0].Length) : new IntPoint();

        public Board(World world)
        {
            this.World = world;
            this.CreateBoard();
        }

        public Board(World world, string[] filecontent)
        {
            this.World = world;

            if (filecontent.Length > 0 && !string.IsNullOrEmpty(filecontent[0]))
            {
                int width = filecontent[0].Count(x => x == '.') + 1;
                int length = filecontent.Length;

                string[][] stringcells = filecontent.Select(x => x.Split(".".ToArray())).ToArray();

                this.Cells = new Cell[width][];
                for (int i = 0; i < width; i++)
                {
                    this.Cells[i] = new Cell[length];
                    for (int j = 0; j < length; j++)
                        this.Cells[i][j] = new Cell(this, stringcells[j][i], i, j);
                }
            }
        }

        public void CreateBoard()
        {
            int width = 30;
            int length = 30;

            string[][] strboard = new string[width][];
            for (int i = 0; i < width; i++)
            {
                strboard[i] = new string[length];
                for (int j = 0; j < length; j++)
                    strboard[i][j] = "";
            }

            //---
            int[] remaining = this.CreateCorners(strboard, width, length, 2);
            this.CreateWaterPoint(strboard, width, length, remaining);
            this.CreateDome(strboard, width, length, null, (5, 9).GetRandom(), null);
            this.CreateDome(strboard, width, length, null, (5, 9).GetRandom(), null);
            this.CreateEmpty(strboard, width, length, "A0");
            this.CreateSmoother(strboard, "A0", "A1", 13);
            this.CreateSmoother(strboard, "A0", "A2", 5);
            this.CreatePlantObject(strboard, "C0", "A0", 0.1d);
            this.CreatePlantObject(strboard, "B2", "A0", 0.02d);
            this.CreatePlantObject(strboard, "B4", "A0", 0.05d);
            this.CreatePlantObject(strboard, "B0", "A0", 0.2d);
            this.CreatePlantObject(strboard, "B3", "A0", 0.01d);
            this.CreatePlantObject(strboard, "B3", "A1", 0.05d);
            this.CreatePlantObject(strboard, "B1", "A0", 0.002d, 1, width / 4);

            this.Cells = new Cell[width][];
            for (int i = 0; i < width; i++)
            {
                this.Cells[i] = new Cell[length];
                for (int j = 0; j < length; j++)
                    this.Cells[i][j] = new Cell(this, String.Join(",", strboard[i][j].Trim().Split(' ')), i, j);
            }
        }

        public IntPoint[] CreateCircle(string[][] strboard, int width, int length, IntPoint coordinate, int radius, string adding, string[] needemptyfor = null)
        {
            List<IntPoint> coords = new List<IntPoint>();

            for (int x = -radius; x <= radius; x++)
            {
                int yt = (int)Math.Floor(Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(x, 2)));
                for (int y = -(yt); y <= yt; y++)
                    if (x + coordinate.X >= 0 && x + coordinate.X < width && y + coordinate.Y >= 0 && y + coordinate.Y < length)
                        if (needemptyfor == null ||
                            (needemptyfor.Length == 1 && needemptyfor[0].Equals("All") && !string.IsNullOrEmpty(strboard[x + coordinate.X][y + coordinate.Y])) ||
                            needemptyfor.All(obj => !strboard[x + coordinate.X][y + coordinate.Y].Contains(obj)))
                        {
                            strboard[x + coordinate.X][y + coordinate.Y] += $" {adding}";
                            coords.Add(new IntPoint(x + coordinate.X, y + coordinate.Y));
                        }
            }

            return coords.ToArray();
        }
        public void CreateDome(string[][] strboard, int width, int length, IntPoint coordinate, int radius, string adding)
        {
            if (coordinate == null)
                coordinate = new IntPoint((0, width + 1).GetRandom(), (0, length + 1).GetRandom());
            if (adding == null)
                adding = $"A{(0, 2).GetRandom()}";

            for (int rad = radius; rad >= 1; rad -= (2, 4).GetRandom())
                this.CreateCircle(strboard, width, length, coordinate, rad, adding, new string[] { "A3" });
        }
        public int[] CreateCorners(string[][] strboard, int width, int length, int times)
        {
            if (times < 1) times = 1;
            if (times > 4) times = 4;

            List<int> remaining = new List<int>() { 0, 1, 2, 3 };

            for (int i = 0; i < times; i++)
            {
                int cornerindex = (0, remaining.Count).GetRandom();
                int corner = remaining[cornerindex];
                remaining.RemoveAt(cornerindex);

                IntPoint coordinate = new IntPoint();
                coordinate.X = (corner == 0 || corner == 3 ? (-4, 5) : (width - 4, width + 5)).GetRandom();
                coordinate.Y = (corner == 0 || corner == 1 ? (-4, 5) : (length - 4, length + 5)).GetRandom();

                int size = (width < length ? width : length) / 2 + 2;

                this.CreateDome(strboard, width, length, coordinate, size, null);
            }

            return remaining.ToArray();
        }

        public void CreateWaterPoint(string[][] strboard, int width, int length, int[] remainingpoints)
        {
            int waterpointlocation = remainingpoints[(0, remainingpoints.Length).GetRandom()];
            int offset = 6;
            IntPoint coordinate =
                waterpointlocation == 0 ? new IntPoint(offset, offset) :
                (waterpointlocation == 1 ? new IntPoint(width - offset, offset) :
                (waterpointlocation == 2 ? new IntPoint(width - offset, length - offset) :
                (waterpointlocation == 3 ? new IntPoint(offset, length - offset) : null)));

            coordinate.X += (-2, 3).GetRandom();
            coordinate.Y += (-2, 3).GetRandom();

            IntPoint[] coords = this.CreateCircle(strboard, width, length, coordinate, (3, 5).GetRandom(), "A3", new string[] { "All" });
            foreach (IntPoint coord in coords)
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (i != 0 || j != 0)
                        {
                            if (coord.X + i >= 0 && coord.X + i < width && coord.Y + j >= 0 && coord.Y + j < length &&
                                string.IsNullOrEmpty(strboard[coord.X + i][coord.Y + j]))
                                strboard[coord.X + i][coord.Y + j] = "A2";
                        }
        }

        public void CreateEmpty(string[][] strboard, int width, int length, string adding)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < length; j++)
                    if (string.IsNullOrEmpty(strboard[i][j]))
                        strboard[i][j] = adding;
        }

        /// <summary>
        /// Poser des objets
        /// </summary>
        /// <param name="strboard"></param>
        /// <param name="adding">Quoi ajouter</param>
        /// <param name="on">Sur quel sol</param>
        /// <param name="probability">Probabilité (de 0 à 1)</param>
        /// <param name="max">Nombre max (0 pour l'infini)</param>
        public void CreatePlantObject(string[][] strboard, string adding, string on, double probability, int max = 0, int offsetFromOuter = 0)
        {
            if (probability < 0d) probability = 0d;
            if (probability > 1d) probability = 1d;

            int already = 0;
            do
            {
                for (int i = offsetFromOuter; i < strboard.Length - offsetFromOuter; i++)
                    for (int j = offsetFromOuter; j < strboard[i].Length - offsetFromOuter; j++)
                    {
                        if (strboard[i][j].EndsWith(on))
                            if ((0, (int)(1d / probability)).GetRandom() == 0)
                            {
                                strboard[i][j] += $" {adding}";
                                if (++already == max) return;
                            }
                    }
            }
            while (max > 0 && already < max);
        }

        public void CreateSmoother(string[][] strboard, string on, string surround, int with)
        {
            List<List<string>> strboardcopy = new List<List<string>>();
            foreach (string[] strline in strboard)
            {
                List<string> newline = new List<string>();
                foreach (string strcase in strline)
                    newline.Add(strcase);
                strboardcopy.Add(newline);
            }

            for (int i = 0; i < strboardcopy.Count; i++)
                for (int j = 0; j < strboardcopy[0].Count; j++)
                {
                    string height = strboardcopy[i][j].Trim().Split(' ').Last();

                    if (height.Equals(on))
                    {
                        int heightindex = strboardcopy[i][j].Trim().Split(' ').Length - 1;
                        int count = 0;

                        for (int k = -1; k <= 1; k++)
                            for (int l = -1; l <= 1; l++)
                            {
                                if ((k != 0 && l == 0) || (l != 0 && k == 0))
                                {
                                    if (i + k >= 0 && i + k < strboardcopy.Count && j + l >= 0 && j + l < strboardcopy[0].Count)
                                    {
                                        string[] heights = strboardcopy[i + k][j + l].Trim().Split(' ');
                                        if (heights.Length >= (heightindex+1))
                                        {
                                            if (heights[heightindex].Equals(surround))
                                            {
                                                if (k == -1 && l == 0) count += 1;
                                                if (k == 0 && l == -1) count += 2;
                                                if (k == 1 && l == 0) count += 4;
                                                if (k == 0 && l == 1) count += 8;
                                            }
                                        }
                                    }
                                }
                            }

                        if (count > 0 && count != 1 && count != 2 && count != 4 && count != 8 && count != 5 && count != 10 && count != 15)
                        {
                            int toplace = with;
                            toplace +=
                                count == 3 ? 0 :
                                (count == 6 ? 1 :
                                (count == 12 ? 2 :
                                (count == 9 ? 3 : 
                                (count == 11 ? 4 : 
                                (count == 7 ? 5 : 
                                (count == 14 ? 6 : 
                                (count == 13 ? 7 : 0
                                )))))));

                            List<string> heights = strboardcopy[i][j].Trim().Split(' ').ToList();
                            heights[heights.Count - 1] = "A"+toplace;
                            strboard[i][j] = string.Join(" ", heights);
                        }
                    }
                }
        }
    }
}
