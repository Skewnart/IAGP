using Seron.Datas;
using Seron.Models;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;

namespace Seron.Windows
{
    public class WorldWindow : RenderWindow
    {
        public World World { get; set; }
        public Text FpsText { get; set; }

        public uint Width { get; set; }
        public uint Height { get; set; }

        public CellSelector Selector { get; set; }
        public Vector2i CameraToGo { get; set; }

        public WorldWindow(VideoMode mode, string title, string worldname, string texturename) : base(mode, title)
        {
            this.Width = mode.Width;
            this.Height = mode.Height;

            this.Selector = new CellSelector(new Texture(Path.Combine(ProgramData.TILESETFOLDER, $"selector.png")));

            this.World = new World(worldname, texturename);

            View view = new View(new FloatRect(0, 0, this.Width, this.Height));
            view.Move(new Vector2f(-this.Width / 2, 0));
            this.SetView(view);

            this.FpsText = new Text()
            {
                Font = new Font("C:/Windows/Fonts/arial.ttf"),
                CharacterSize = 20,
                FillColor = Color.White
            };

            this.Closed += Window_Closed;
            this.KeyPressed += WorldWindow_KeyPressed;
            this.MouseMoved += WorldWindow_MouseMoved;
            this.MouseEntered += WorldWindow_MouseEntered;
            this.MouseLeft += WorldWindow_MouseLeft;
            this.MouseButtonPressed += WorldWindow_MouseButtonPressed;
        }

        private void WorldWindow_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            this.CameraToGo = new Vector2i(e.X, e.Y);
        }

        private void WorldWindow_MouseLeft(object sender, EventArgs e)
        {
            this.Selector.NeedToShow = false;
        }

        private void WorldWindow_MouseEntered(object sender, EventArgs e)
        {
            this.Selector.NeedToShow = true;
        }

        private void WorldWindow_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            Vector2f viewCenter = this.GetView().Center;
            //Le point calculé par rapport à la caméra
            Vector2f realPoint = new Vector2f(viewCenter.X - this.Width / 2 + e.X, viewCenter.Y - this.Height / 2 + e.Y);

            //De quelles parties du losange le curseur fait parti
            bool firstPartX = (Math.Abs(realPoint.X % (Cell.WIDTHOFFSET * 2)) < Cell.WIDTHOFFSET) == (realPoint.X >= 0);
            bool firstPartY = (Math.Abs(realPoint.Y % (Cell.LENGTHOFFSET * 2)) < Cell.LENGTHOFFSET) == (realPoint.Y >= 0);

            //Le point local (par rapport à la partie)
            int localX = realPoint.X < 0 ? (Cell.WIDTHOFFSET - (int)Math.Abs(realPoint.X % Cell.WIDTHOFFSET)) : (int)Math.Abs(realPoint.X % Cell.WIDTHOFFSET);
            int localY = realPoint.Y < 0 ? (Cell.LENGTHOFFSET - (int)Math.Abs(realPoint.Y % Cell.LENGTHOFFSET)) : (int)Math.Abs(realPoint.Y % Cell.LENGTHOFFSET);

            //Offset par rapport au nouveau point local, si besoin
            int isometricXOffset = 0, isometricYOffset = 0;
            if (firstPartX == firstPartY)
                isometricXOffset = (firstPartX && localY < Cell.WIDTHOFFSET / 2 - localX / 2) ? -1 : ((!firstPartX && localY > Cell.WIDTHOFFSET / 2 - localX / 2) ? 1 : 0);
            else
                isometricYOffset = (firstPartX && localY > localX / 2) ? 1 : ((!firstPartX && localY < localX / 2) ? -1 : 0);

            //On récupère le point précis du carré pour le convertir en isométrique
            realPoint.X = ((int)(realPoint.X / (Cell.WIDTHOFFSET * 2)) - (realPoint.X < 0 ? 1 : 0)) * Cell.WIDTHOFFSET * 2;
            realPoint.Y = ((int)(realPoint.Y / (Cell.LENGTHOFFSET * 2)) - (realPoint.Y < 0 ? 1 : 0)) * Cell.LENGTHOFFSET * 2;

            //Calcul du point isométrique + offset
            Vector2f isometricPoint = new Vector2f();
            isometricPoint.X = (realPoint.Y / 16 + realPoint.X / 16 / 2) / 2;
            isometricPoint.Y = realPoint.Y / 16 - isometricPoint.X + isometricYOffset;
            isometricPoint.X += isometricXOffset;

            this.Selector.ChangePosition(this.World.Board.Size, new IntPoint((int)isometricPoint.X, (int)isometricPoint.Y));
        }

        private void WorldWindow_KeyPressed(object sender, KeyEventArgs e)
        {
            int speed = 8;
            int x = 0, y = 0;
            if (e.Code == Keyboard.Key.Up)
                y -= speed;
            else if (e.Code == Keyboard.Key.Down)
                y += speed;
            if (e.Code == Keyboard.Key.Left)
                x -= speed;
            else if (e.Code == Keyboard.Key.Right)
                x += speed;

            if (x != 0 || y != 0)
            {
                View view = this.GetView();
                view.Move(new Vector2f(x, y));
                this.SetView(view);
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ((RenderWindow)sender).Close();
        }

        public void Start()
        {
            Clock clock = new Clock();
            long elapsed = 0;

            while (this.IsOpen)
            {
                long now = clock.ElapsedTime.AsMicroseconds();
                if (1000000 / (now - elapsed) <= 60)
                {
                    int currentFPS = (int)(1000000 / (now - elapsed));
                    elapsed = now;

                    this.DispatchEvents();

                    Vector2f center = this.GetView().Center;
                    this.FpsText.DisplayedString = currentFPS.ToString();
                    this.FpsText.Position = new Vector2f(center.X - this.Width / 2, center.Y - this.Height / 2);

                    this.Clear();
                    this.DrawWorld();
                    this.Draw(FpsText);
                    this.Display();
                }
            }
        }

        private void DrawWorld()
        {
            View view = this.GetView();

            if (this.CameraToGo.X != 0 || this.CameraToGo.Y != 0)
            {
                double distance = Math.Sqrt(Math.Pow(this.CameraToGo.X - this.Width / 2, 2) + Math.Pow(this.CameraToGo.Y - this.Height / 2, 2));
                if (distance < 8)
                {
                    view.Move(new Vector2f(this.CameraToGo.X - this.Width / 2, this.CameraToGo.Y - this.Height / 2));
                    this.CameraToGo = new Vector2i(0, 0);
                }
                else
                {
                    double percent = 8 / distance;
                    view.Move(new Vector2f((float)(percent * (this.CameraToGo.X - this.Width / 2)), (float)(percent * (this.CameraToGo.Y - this.Height / 2))));
                    this.CameraToGo = new Vector2i((int)(this.CameraToGo.X - percent * (this.CameraToGo.X - this.Width / 2)), (int)(this.CameraToGo.Y - percent * (this.CameraToGo.Y - this.Height / 2)));
                }

                this.SetView(view);
            }

            for (int i = 0; i < this.World.Board.Cells.Length; i++)
                for (int j = 0; j < this.World.Board.Cells[i].Length; j++)
                    foreach (CellHeight height in this.World.Board.Cells[i][j].CellsHeight)
                        if (height.FinalPosition.X > view.Center.X - this.Width / 2 - Cell.CUBEWIDTH && height.FinalPosition.X < view.Center.X + this.Width / 2
                            && height.FinalPosition.Y > view.Center.Y - this.Height / 2 - Cell.CUBEWIDTH && height.FinalPosition.Y < view.Center.Y + this.Height / 2)
                        {
                            this.World.Board.Cells[i][j].Draw(this, (i == this.Selector.IsometricPosition.X && j == this.Selector.IsometricPosition.Y) ? this.Selector : null);
                            break;
                        }
        }
    }
}
