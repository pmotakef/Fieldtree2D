using System;
using System.Collections.Generic;
using System.Linq;
using FieldTreeStructure.Geometry;
using FieldTreeStructure.Spatial;

namespace FieldTreeStructure.Node.Partition
{
    public class PartitionNode<T> : AbstractNode<T>, IEquatable<PartitionNode<T>> where T : ISpatial
    {
        protected Rectangle Bounds;

        protected List<PartitionNode<T>> Children = new List<PartitionNode<T>>();
        protected List<PartitionNode<T>> Parents = new List<PartitionNode<T>>();

        public PartitionNode(Rectangle bounds, int capacity, int layer, List<PartitionNode<T>> parents)
        {
            Bounds = bounds;
            Capacity = capacity;
            LayerNum = layer;
            Parents = parents;
        }

        public bool Equals(PartitionNode<T> other)
        {
            return (LayerNum == other.LayerNum && Bounds.Equals(other.Bounds));
        }

        public List<PartitionNode<T>> GetChildren()
        {
            return Children;
        }

        public bool HasChildren()
        {
            return Children.Count > 0;
        }

        public List<PartitionNode<T>> GetParent()
        {
            return Parents;
        }

        public override bool IsRootNode()
        {
            return (Parents.Count < 1);
        }

        public void AddParent(PartitionNode<T> parent)
        {
            if (!Parents.Contains(parent))
            {
                Parents.Add(parent);
            }
        }

        public Rectangle GetBounds()
        {
            return Bounds;
        }

        public bool IsNodeNeighbor(PartitionNode<T> node)
        {
            int dx = Math.Abs(node.GetBounds().Center.X - Bounds.Center.X);
            int dy = Math.Abs(node.GetBounds().Center.Y - Bounds.Center.Y);
            return dx <= Bounds.Width && dx > 0 && dy <= Bounds.Height && dy > 0;
        }

        public List<PartitionNode<T>> FindSiblings()
        {
            List<PartitionNode<T>> siblings = new List<PartitionNode<T>>();
            foreach (PartitionNode<T> parent in Parents)
            {
                siblings.AddRange(parent.GetChildren().Where(x => (x.IsNodeNeighbor(this))));
            }
            return siblings.Distinct().ToList();
        }

        public List<PartitionNode<T>> FindExistingChildren(List<PartitionNode<T>> siblings = null)
        {
            List<PartitionNode<T>> existingChildren = new List<PartitionNode<T>>();
            siblings = siblings ?? FindSiblings();

            foreach (PartitionNode<T> sibling in siblings)
            {
                existingChildren.AddRange(sibling.GetChildren().Where(x => x.GetBounds().IntersectsWith(Bounds)));
            }
            return existingChildren.Distinct().ToList();
        }

        public void CreateChildren()
        {
            // Do nothing, if children are already created
            if (Children.Count > 1)
                return;

            // Adding exisitng children
            List<PartitionNode<T>> siblings = FindSiblings();
            Children.AddRange(FindExistingChildren(siblings).Where(x => !Children.Contains(x)));

            int children_layer = LayerNum + 1;
            Size children_size = new Size(Bounds.Width / 2, Bounds.Height / 2);
            List<PartitionNode<T>> parent = new List<PartitionNode<T>> { this };

            int nodeX = Bounds.Center.X - (Bounds.Width / 2);
            int nodeY = Bounds.Center.Y - (Bounds.Height / 2);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int centX = nodeX + ((Bounds.Width / 2) * i);
                    int centY = nodeY + ((Bounds.Height / 2) * j);
                    PartitionNode<T> new_child = new PartitionNode<T>(new Rectangle(new Point(centX, centY), children_size), Capacity, children_layer, parent);
                    if (!Children.Contains(new_child))
                    {
                        Children.Add(new_child);
                    }
                }
            }
            // Updating all known parents
            foreach (var child in Children)
            {
                foreach (var par in siblings.Where(x => x.IntersectsWith(child.GetBounds())))
                {
                    child.AddParent(par);
                }
            }
        }

        public void ClearChildren()
        {
            foreach (var sibling in FindSiblings())
            {
                foreach (var s_child in sibling.GetChildren())
                {
                    if (Children.Contains(s_child))
                    {
                        sibling.RemoveChild(s_child);
                    }
                }
            }
            Children.Clear();
        }

        public void RemoveChild(PartitionNode<T> child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
            }
        }

        public void MergeEmptyChildren()
        {
            foreach (var sibling in FindCenterSibling())
            {
                if (sibling.AreExistingChildrenEmpty())
                {
                    sibling.ClearChildren();
                }
            }
        }

        public List<PartitionNode<T>> FindCenterSibling()
        {
            // The case current node is the center node
            if ((Parents.Count == 1 && Parents[0].GetBounds().ContainsRect(Bounds)) || IsRootNode())
            {
                return new List<PartitionNode<T>>() { this };
            }
            List<PartitionNode<T>> centerSiblings = new List<PartitionNode<T>>();
            foreach (var parent in Parents)
            {
                centerSiblings.AddRange(parent.GetChildren().Where(x => parent.GetBounds().ContainsRect(x.GetBounds())));
            }
            return centerSiblings;
        }

        public bool AreExistingChildrenEmpty()
        {
            if (Children.Count < 1)
            {
                return false;
            }
            foreach (PartitionNode<T> child in Children)
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
            return Bounds;
        }

        public override Rectangle GetOperatingBounds()
        {
            return Bounds;
        }
    }
}
