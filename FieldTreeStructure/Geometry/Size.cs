using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldTreeStructure.Geometry
{
    public struct Size : IComparable<Size>, IEquatable<Size>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public Size(Point p)
        {
            Width = p.X;
            Height = p.Y;
        }

        public int CompareTo(Size other)
        {
            if (Width.CompareTo(other.Width) == 0)
            {
                return (Height.CompareTo(other.Height));
            }
            return (Width.CompareTo(other.Width));
        }

        public bool Equals(Size other)
        {
            return (Width.Equals(other.Width) && Height.Equals(other.Height));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
                return false;
            Size other = (Size)obj;
            return (Width.Equals(other.Width) && Height.Equals(other.Height));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 29 + Width.GetHashCode();
                hash = hash * 29 + Height.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} x {1}", Width, Height);
        }

    }
}
