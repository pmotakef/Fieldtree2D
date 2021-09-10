using FieldTree2D_v2.Geometry;

namespace FieldTree2D_v2.Spatial
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
