using System;
using System.Collections.Generic;
using System.Linq;
using FieldTreeStructure.Geometry;
using FieldTreeStructure.Spatial;
using FieldTreeStructure.Utilities;

namespace FieldTreeStructure.Node.Cover
{
    public class CoverNode<T> : IEquatable<CoverNode<T>> where T : ISpatial
    {
        /*      Childern Positions
        *      ---------------------------------
        *      |       0       |       1       |
        *      ---------------------------------
        *      |       2       |       3       |
        *      ---------------------------------
        */
        protected double pVal;
        protected Rectangle ActualBounds;
        protected Rectangle OperatingBounds;

        protected List<CoverNode<T>> Children = new List<CoverNode<T>>();
        protected CoverNode<T> Parent;

        protected int Capacity;
        protected int LayerNum;

        protected List<SpatialObj<T>> StoredObjs = new List<SpatialObj<T>>();
        protected List<SpatialObj<T>> OverflowObjs = new List<SpatialObj<T>>();

        public CoverNode(Rectangle bounds, int capacity, int layer, double p_value, CoverNode<T> parent)
        {
            ActualBounds = bounds;
            Capacity = capacity;
            LayerNum = layer;
            pVal = p_value;
            OperatingBounds = new Rectangle(ActualBounds.Center, (int)(ActualBounds.Width * (1.0 + pVal)), (int)(ActualBounds.Height * (1.0 + pVal))); ;
            Parent = parent;
        }

        public bool Equals(CoverNode<T> other)
        {
            return (LayerNum == other.LayerNum && ActualBounds.Equals(other.ActualBounds));
        }

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

        public List<CoverNode<T>> GetChildren()
        {
            return Children;
        }

        public bool HasChildren()
        {
            return Children.Count > 0;
        }

        public CoverNode<T> GetParent()
        {
            return Parent;
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

        public bool IsRootNode()
        {
            return (Parent == null);
        }

        public bool IsPointInNode(Point p)
        {
            return ActualBounds.ContainsPoint(p);
        }

        public bool IntersectsWith(Rectangle rectangle)
        {
            return OperatingBounds.IntersectsWith(rectangle);
        }

        public bool IntersectsWith(Point center, int rad)
        {
            return OperatingBounds.IntersectsCircle(center, rad);
        }

        public double GetDistanceToPointSq(Point p)
        {
            return OperatingBounds.GetDistanceSqToPoint(p);
        }

        public bool IsObjectInNode(SpatialObj<T> obj)
        {
            return OperatingBounds.ContainsRect(obj.boundingBox);
        }

        public int GetMaxDistance()
        {
            return OperatingBounds.Width + OperatingBounds.Height;
        }

        public List<SpatialObj<T>> GetOverflowObjs()
        {
            return OverflowObjs;
        }

        public bool CanSink(SpatialObj<T> rect)
        {
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

        public bool DeleteRectangle(SpatialObj<T> rect, bool overflow_only = false)
        {
            if (!overflow_only)
            {
                StoredObjs.RemoveAll(x => x.objInstance.Equals(rect.objInstance));
            }
            OverflowObjs.RemoveAll(x => x.objInstance.Equals(rect.objInstance));
            return IsOverflown();
        }

        public bool DeleteRectangles(List<SpatialObj<T>> rects, bool overflow_only = false)
        {
            foreach (SpatialObj<T> rect in rects)
            {
                DeleteRectangle(rect, overflow_only);
            }
            return IsOverflown();
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

        public void CreateChildren()
        {
            // Do nothing, if children are already created
            if (Children.Count > 1)
                return;

            int children_layer = LayerNum + 1;
            Size children_size = new Size(ActualBounds.Width / 2, ActualBounds.Height / 2);

            int x1 = ActualBounds.Center.X - (children_size.Width / 2);
            int x2 = ActualBounds.Center.X + (children_size.Width / 2);
            int y1 = ActualBounds.Center.Y - (children_size.Height / 2);
            int y2 = ActualBounds.Center.Y + (children_size.Height / 2);

            Children.Add(new CoverNode<T>(new Rectangle(new Point(x1, y1), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
            Children.Add(new CoverNode<T>(new Rectangle(new Point(x2, y1), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
            Children.Add(new CoverNode<T>(new Rectangle(new Point(x1, y2), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
            Children.Add(new CoverNode<T>(new Rectangle(new Point(x2, y2), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
        }

        public void MergeEmptyChildren()
        {
            Children.Clear();
        }

        public bool AreExistingChildrenEmpty()
        {
            if (Children.Count < 1)
            {
                return false;
            }
            foreach (CoverNode<T> child in Children)
            {
                if (child.HasChildren() || !child.IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
