using System;
using Microsoft.Xna.Framework;

namespace MiniJam61Egypt.Util
{
    public struct BoundingBox2D
    {
        public static readonly BoundingBox2D Empty = new BoundingBox2D(0, 0, 0, 0);

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2 Center
        {
            get => new Vector2(X + Width / 2f, Y + Height / 2f);
            set { X = value.X - Width / 2f; Y = value.Y - Height / 2f; }
        }
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Vector2 TopLeft
        {
            get => new Vector2(X, Y);
            set { X = value.X; Y = value.Y; }
        }
        public Vector2 TopRight
        {
            get => new Vector2(Right, Y);
            set { X = value.X - Width; Y = value.Y; }
        }
        public Vector2 BottomLeft
        {
            get => new Vector2(X, Bottom);
            set { X = value.X; Y = value.Y - Height; }
        }
        public Vector2 BottomRight
        {
            get => new Vector2(Right, Bottom);
            set { X = value.X - Width; Y = value.Y - Height; }
        }

        public float Left { get => X; }
        public float Right { get => X + Width; }
        public float Top { get => Y; }
        public float Bottom { get => Y + Height; }

        public BoundingBox2D(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public BoundingBox2D(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public BoundingBox2D(Rectangle rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public BoundingBox2D Move(Vector2 amount)
        {
            return new BoundingBox2D(X + amount.X, Y + amount.Y, Width, Height);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)Math.Round(X), (int)Math.Round(Y), (int)Width, (int)Height);
        }

        public void MoveMe(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public bool Intersects(Rectangle value)
        {
            return Left < value.Right && Right > value.Left && Top < value.Bottom && Bottom > value.Top;
        }

        public bool Intersects(BoundingBox2D value)
        {
            return Left < value.Right && Right > value.Left && Top < value.Bottom && Bottom > value.Top;
        }

        public BoundingBox2D Interpenetration(Rectangle rect)
        {
            float x1 = Math.Max(X, rect.X);
            float x2 = Math.Min(Right, rect.X + rect.Width);
            float y1 = Math.Max(Y, rect.Y);
            float y2 = Math.Min(Bottom, rect.Y + rect.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new BoundingBox2D(x1, y1, x2 - x1, y2 - y1);
            }

            return Empty;
        }

        public BoundingBox2D Interpenetration(BoundingBox2D rect)
        {
            float x1 = Math.Max(X, rect.X);
            float x2 = Math.Min(Right, rect.X + rect.Width);
            float y1 = Math.Max(Y, rect.Y);
            float y2 = Math.Min(Bottom, rect.Y + rect.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new BoundingBox2D(x1, y1, x2 - x1, y2 - y1);
            }

            return Empty;
        }

        public BoundingBox2D Union(BoundingBox2D box1, BoundingBox2D box2)
        {
            float left = Math.Min(box1.X, box2.X);
            float top = Math.Min(box1.Y, box2.Y);

            float right = Math.Max(box1.Right, box2.Right);
            float bottom = Math.Max(box1.Bottom, box2.Bottom);

            return new BoundingBox2D(left, top, right - left, bottom - top);
        }

        public override bool Equals(object obj)
        {
            if (obj is BoundingBox2D bbox)
            {
                return bbox.X == X && bbox.Y == Y && bbox.Width == Width && bbox.Height == Height;
            }
            return false;
        }

        public static bool operator ==(BoundingBox2D b1, BoundingBox2D b2)
        {
            return b1.Equals(b2);
        }

        public static bool operator !=(BoundingBox2D b1, BoundingBox2D b2)
        {
            return !b1.Equals(b2);
        }
    }
}
