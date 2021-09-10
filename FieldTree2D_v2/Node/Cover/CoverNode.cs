using FieldTree2D_v2.Geometry;
using FieldTree2D_v2.Spatial;


namespace FieldTree2D_v2.Node.Cover
{
    public class CoverNode<T> : AbstractNode<T>, IEquatable<CoverNode<T>> where T : ISpatial
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

        public override bool IsRootNode()
        {
            return (Parent == null);
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

        public override Rectangle GetActualBounds()
        {
            return ActualBounds;
        }

        public override Rectangle GetOperatingBounds()
        {
            return OperatingBounds;
        }
    }
}
