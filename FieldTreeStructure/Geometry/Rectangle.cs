using System;
using System.Collections.Generic;
using FieldTreeStructure.Spatial;

namespace FieldTreeStructure.Geometry
{
    public struct Rectangle : IEquatable<Rectangle>
    {

        public Point Center { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private const int TWO = 2;

        public Rectangle(Point center, int w, int h)
        {
            Width = w;
            Height = h;
            Center = center;
        }

        public Rectangle(Point center, Size size)
        {
            Width = size.Width;
            Height = size.Height;
            Center = center;
        }

        public Rectangle(Point p1, Point p2)
        {
            Center = new Point((p1.X + p2.X) / TWO, (p1.Y + p2.Y) / TWO);
            Width = Math.Abs(p1.X - p2.X);
            Height = Math.Abs(p1.Y - p2.Y);
        }

        public Rectangle(ISpatial obj)
        {
            Center = new Point(obj.CenterX, obj.CenterY);
            Width = obj.Width;
            Height = obj.Height;
        }

        public Point GetTwiceMaxExtent()
        {
            return new Point((2 * Center.X) + Width, (2 * Center.Y) + Height);
        }

        public Point GetTwiceMinExtent()
        {
            return new Point((2 * Center.X) - Width, (2 * Center.Y) - Height);
        }

        public IEnumerable<Point> GetCorners()
        {
            Point minExt = GetTwiceMinExtent();
            Point maxExt = GetTwiceMaxExtent();
            List<Point> corners = new List<Point>();
            corners.Add(new Point(minExt.X, minExt.Y));
            corners.Add(new Point(minExt.X, maxExt.Y));
            corners.Add(new Point(maxExt.X, minExt.Y));
            corners.Add(new Point(maxExt.X, maxExt.Y));
            return corners;
        }

        public bool Equals(Rectangle other)
        {
            return (Center.Equals(other.Center) && Width == other.Width && Height == other.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;
            Rectangle other = (Rectangle)obj;
            return (Center.Equals(other.Center) && Width == other.Width && Height == other.Height);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Center.X.GetHashCode();
                hash = hash * 23 + Center.Y.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + Height.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            Point minExt = GetTwiceMinExtent();
            Point maxExt = GetTwiceMaxExtent();
            return string.Format("({0:0.0}, {1:0.0}) - ({2:0.0}, {3:0.0})", minExt.X / 2.0f, minExt.Y / 2.0f, maxExt.X / 2.0f, maxExt.Y / 2.0f);
        }

        public bool ContainsRect(Rectangle other)
        {
            return other.ContainedByRect(this);
        }

        public bool ContainedByRect(Rectangle other)
        {
            Point thisMinExt = GetTwiceMinExtent();
            Point thisMaxExt = GetTwiceMaxExtent();
            Point otherMinExt = other.GetTwiceMinExtent();
            Point otherMaxExt = other.GetTwiceMaxExtent();
            return (thisMaxExt.X <= otherMaxExt.X) && (thisMaxExt.Y <= otherMaxExt.Y) && (thisMinExt.X >= otherMinExt.X) && (thisMinExt.Y >= otherMinExt.Y);
        }

        public bool ContainsPoint(Point p)
        {
            Point minExt = GetTwiceMinExtent();
            Point maxExt = GetTwiceMaxExtent();
            return (2 * p.X <= maxExt.X && 2 * p.X >= minExt.X && 2 * p.Y <= maxExt.Y && 2 * p.Y >= minExt.Y);
        }

        public double GetDistanceSqToPoint(Point p)
        {
            double dx = Math.Max(Math.Abs(p.X - Center.X) - Width / 2.0, 0.0);
            double dy = Math.Max(Math.Abs(p.Y - Center.Y) - Height / 2.0, 0.0);
            return ((dx * dx) + (dy * dy));
        }

        public bool ContainedByCircle(Point center, int radius)
        {
            foreach (Point corner in GetCorners())
            {
                if (Point.CalcDistSq(center, new Point(corner.X, corner.Y)) > 4 * radius * radius)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IntersectsCircle(Point center, int radius)
        {
            double dist = GetDistanceSqToPoint(center);
            return ((int)dist <= radius * radius);
        }

        public static bool IntersectionCheck(Rectangle rectA, Rectangle rectB)
        {
            var minExt_a = rectA.GetTwiceMinExtent();
            var maxExt_a = rectA.GetTwiceMaxExtent();
            var minExt_b = rectB.GetTwiceMinExtent();
            var maxExt_b = rectB.GetTwiceMaxExtent();
            return (minExt_a.X <= maxExt_b.X && maxExt_a.X >= minExt_b.X && minExt_a.Y <= maxExt_b.Y && maxExt_a.Y >= minExt_b.Y);
        }

        public bool IntersectsWith(Rectangle other)
        {
            return IntersectionCheck(this, other);
        }

    }
}
