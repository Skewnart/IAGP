using SFML.Graphics;
using SFML.System;

namespace Seron.Models
{
    public class CellSelector : Sprite
    {
        public bool NeedToShow { get; set; } = false;
        public IntPoint IsometricPosition { get; set; }

        public CellSelector(Texture texture)
        {
            this.Texture = texture;
            this.IsometricPosition = new IntPoint();
        }

        public void ChangePosition(IntPoint size, IntPoint isometricPosition, bool changeNeedToShow = true)
        {
            if (isometricPosition.X >= 0 && isometricPosition.X <= size.X && isometricPosition.Y >= 0 && isometricPosition.Y <= size.Y) {
                if (changeNeedToShow)
                    this.NeedToShow = true;

                this.IsometricPosition.X = isometricPosition.X;
                this.IsometricPosition.Y = isometricPosition.Y;

                this.Position = new Vector2f((float)((isometricPosition.X - isometricPosition.Y) * Cell.WIDTHOFFSET), (float)((isometricPosition.X + isometricPosition.Y) * Cell.LENGTHOFFSET));

            }
            else {
                if (changeNeedToShow)
                    this.NeedToShow = false;
            }
        }

        public void Draw(RenderWindow window)
        {
            if (this.NeedToShow) {
                window.Draw(this);
            }
        }
    }

    public class IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IntPoint(int x = 0, int y = 0)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
