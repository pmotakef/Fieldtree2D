
namespace FieldTree2D_v2.Geometry
{
    public struct Point : IComparable<Point>, IEquatable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Size s)
        {
            X = s.Width;
            Y = s.Height;
        }

        public int CompareTo(Point other)
        {
            if (X.CompareTo(other.X) == 0)
            {
                return (Y.CompareTo(other.Y));
            }
            return (X.CompareTo(other.X));
        }

        public bool Equals(Point other)
        {
            return (X.Equals(other.X) && Y.Equals(other.Y));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;
            Point other = (Point)obj;
            return (X.Equals(other.X) && Y.Equals(other.Y));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 29 + X.GetHashCode();
                hash = hash * 29 + Y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

        public static double CalcDistSq(Point p1, Point p2)
        {
            return ((p1.X - p2.X) * (p1.X - p2.X)) + ((p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}
