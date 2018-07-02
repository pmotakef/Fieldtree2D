using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldTreeStructure.Geometry;

namespace FieldTreeStructure.Spatial
{
    public interface ISpatial : IEquatable<ISpatial>
    {
        int CenterX { get; }
        int CenterY { get; }
        int Width { get; }
        int Height { get; }
    }

    public interface ISpatialF : IEquatable<ISpatialF>
    {
        float CenterX { get; }
        float CenterY { get; }
        float Width { get; }
        float Height { get; }
    }

    public struct SpatialObj<T> where T : ISpatial
    {
        public T objInstance;
        public Rectangle boundingBox;

        public SpatialObj(T obj)
        {
            objInstance = obj;
            boundingBox = new Rectangle(obj);
        }
    }
}
