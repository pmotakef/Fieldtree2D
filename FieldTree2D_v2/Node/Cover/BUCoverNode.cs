
using FieldTree2D_v2.Geometry;
using FieldTree2D_v2.Spatial;

namespace FieldTree2D_v2.Node.Cover
{
    public class BUCoverNode<T> : AbstractNode<T>, IEquatable<BUCoverNode<T>> where T : ISpatial
    {

        protected double pVal;
        protected Rectangle ActualBounds;
        protected Rectangle OperatingBounds;

        protected List<BUCoverNode<T>> Children = new List<BUCoverNode<T>>();
        protected BUCoverNode<T> Parent;

        public BUCoverNode(Rectangle bounds, int capacity, int layer, double p_value, BUCoverNode<T> parent)
        {
            ActualBounds = bounds;
            Capacity = capacity;
            LayerNum = layer;
            pVal = p_value;
            OperatingBounds = new Rectangle(ActualBounds.Center, (int)(ActualBounds.Width * (1.0 + pVal)), (int)(ActualBounds.Height * (1.0 + pVal)));
            Parent = parent;
        }

        public bool Equals(BUCoverNode<T> other)
        {
            return (LayerNum == other.LayerNum && ActualBounds.Equals(other.ActualBounds));
        }

        public override Rectangle GetActualBounds()
        {
            return ActualBounds;
        }

        public override Rectangle GetOperatingBounds()
        {
            return OperatingBounds;
        }

        public override bool IsRootNode()
        {
            return Parent == null;
        }

        public List<BUCoverNode<T>> GetChildren()
        {
            return Children;
        }

        public bool HasChildren()
        {
            return Children.Count > 0;
        }

        public BUCoverNode<T> GetParent()
        {
            return Parent;
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

            AddChild(new BUCoverNode<T>(new Rectangle(new Point(x1, y1), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
            AddChild(new BUCoverNode<T>(new Rectangle(new Point(x2, y1), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
            AddChild(new BUCoverNode<T>(new Rectangle(new Point(x1, y2), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
            AddChild(new BUCoverNode<T>(new Rectangle(new Point(x2, y2), children_size.Width, children_size.Height), Capacity, children_layer, pVal, this));
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
            foreach (BUCoverNode<T> child in Children)
            {
                if (child.HasChildren() || !child.IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        private void AddChild(BUCoverNode<T> child)
        {
            if (!Children.Contains(child))
            {
                Children.Add(child);
            }
        }

        public BUCoverNode<T> CreateParent(Point p)
        {
            if (ActualBounds.ContainsPoint(p))
            {
                return (this);
            }
            Point maxExt = ActualBounds.GetTwiceMaxExtent();
            maxExt.X /= 2;
            maxExt.Y /= 2;
            Point minExt = ActualBounds.GetTwiceMinExtent();
            minExt.X /= 2;
            minExt.Y /= 2;

            Point center = new Point();
            if (p.X >= minExt.X && p.Y >= maxExt.Y)
            {
                center.X = maxExt.X;
                center.Y = maxExt.Y;
            }
            else if (p.X < minExt.X && p.Y >= minExt.Y)
            {
                center.X = minExt.X;
                center.Y = maxExt.Y;
            }
            else if (p.X < maxExt.X && p.Y < minExt.Y)
            {
                center.X = minExt.X;
                center.Y = minExt.Y;
            }
            else
            {
                center.X = maxExt.X;
                center.Y = minExt.Y;
            }

            BUCoverNode<T> new_root = new BUCoverNode<T>(new Rectangle(center, ActualBounds.Width * 2, ActualBounds.Height * 2), Capacity, LayerNum - 1, pVal, null);
            new_root.AddChild(this);
            new_root.CreateChildren();
            Parent = new_root;

            return new_root;
        }

    }
}
