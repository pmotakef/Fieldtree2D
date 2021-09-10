using FieldTree2D_v2.Geometry;
using FieldTree2D_v2.Spatial;
using FieldTree2D_v2.Utilities;

namespace FieldTree2D_v2.Node
{
    public abstract class AbstractNode<T> where T : ISpatial
    {
        protected int Capacity;
        protected int LayerNum;

        protected List<SpatialObj<T>> StoredObjs = new List<SpatialObj<T>>();
        protected List<SpatialObj<T>> OverflowObjs = new List<SpatialObj<T>>();

        public List<SpatialObj<T>> GetAllObjects()
        {
            List<SpatialObj<T>> allObjs = new List<SpatialObj<T>>(StoredObjs);
            allObjs.AddRange(OverflowObjs);
            return allObjs;
        }

        public void Clear()
        {
            OverflowObjs.Clear();
            StoredObjs.Clear();
        }

        public int Count()
        {
            return (StoredObjs.Count + OverflowObjs.Count);
        }

        public bool IsFull()
        {
            return (Count() >= Capacity);
        }

        public bool IsEmpty()
        {
            return (Count() == 0);
        }

        public bool IsOverflown()
        {
            return (OverflowObjs.Count > 0);
        }

        public List<SpatialObj<T>> GetOverflowObjs()
        {
            return OverflowObjs;
        }

        public bool DeleteRectangle(SpatialObj<T> rect, bool overflow_only = false)
        {
            int oldCount = Count();
            if (!overflow_only)
            {
                StoredObjs.RemoveAll(x => x.objInstance.Equals(rect.objInstance));
            }
            OverflowObjs.RemoveAll(x => x.objInstance.Equals(rect.objInstance));
            return oldCount > Count();
        }

        public int DeleteRectangles(List<SpatialObj<T>> rects, bool overflow_only = false)
        {
            int numDeleted = 0;
            foreach (SpatialObj<T> rect in rects)
            {
                if (DeleteRectangle(rect, overflow_only))
                {
                    numDeleted += 1;
                }
            }
            return numDeleted;
        }

        public List<SpatialObj<T>> GetRangeQueryObj(Point c, int r)
        {
            return GetAllObjects().Where(x => x.boundingBox.ContainedByCircle(c, r)).ToList();
        }

        public List<SpatialObj<T>> GetRangeQueryObj(Rectangle window)
        {
            return GetAllObjects().Where(x => x.boundingBox.ContainedByRect(window)).ToList();
        }

        public Dictionary<SpatialObj<T>, double> GetNearestRectangle(Point p)
        {
            return Utils.MinOrUpperBound(GetAllObjects(), x => x.boundingBox.GetDistanceSqToPoint(p), Statics.EPSILON);
        }

        public bool IntersectsWith(Rectangle rectangle)
        {
            return GetOperatingBounds().IntersectsWith(rectangle);
        }

        public bool IntersectsWith(Point center, int rad)
        {
            return GetOperatingBounds().IntersectsCircle(center, rad);
        }

        public double GetDistanceToPointSq(Point p)
        {
            return GetOperatingBounds().GetDistanceSqToPoint(p);
        }

        public bool IsObjectInNode(SpatialObj<T> obj)
        {
            return GetOperatingBounds().ContainsRect(obj.boundingBox);
        }

        public int GetMaxDistance()
        {
            return GetOperatingBounds().Width + GetOperatingBounds().Height;
        }

        public bool CanSink(SpatialObj<T> rect)
        {
            Rectangle OperatingBounds = GetOperatingBounds();
            Rectangle ActualBounds = GetActualBounds();
            return (rect.boundingBox.Width <= OperatingBounds.Width / 2) && (rect.boundingBox.Height <= OperatingBounds.Height / 2) &&
                Math.Min(ActualBounds.Height, ActualBounds.Width) > 1;
        }

        public bool StoreRectangle(SpatialObj<T> rect)
        {
            if (CanSink(rect))
            {
                OverflowObjs.Add(rect);
            }
            else
            {
                StoredObjs.Add(rect);
            }
            return (IsOverflown());
        }

        public bool IsPointInNode(Point p)
        {
            return GetActualBounds().ContainsPoint(p);
        }

        public abstract Rectangle GetActualBounds();
        public abstract Rectangle GetOperatingBounds();

        public abstract bool IsRootNode();
    }
}
