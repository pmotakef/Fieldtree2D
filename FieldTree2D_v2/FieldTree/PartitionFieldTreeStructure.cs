using FieldTree2D_v2.Geometry;
using FieldTree2D_v2.Spatial;
using FieldTree2D_v2.Node.Partition;
using Priority_Queue;

namespace FieldTree2D_v2.FieldTree
{
    public class PartitionFieldTreeStructure<T> where T : ISpatial
    {
        /// <summary>
        /// A generic class for NodeType (can be either Cover or Partition Fieldtree node).
        /// </summary>
        protected class NodeOrObj : IEquatable<NodeOrObj>
        {
            /// <summary>
            /// A Private wrapper class for Nodes and Rectangle objects since both have RectangleObj part.
            /// </summary>
            private PartitionNode<T> node;
            private SpatialObj<T> obj;
            bool isNode;

            public NodeOrObj() { }

            public void SetNode(PartitionNode<T> n)
            {
                node = n;
                isNode = true;
            }

            public void SetObj(SpatialObj<T> o)
            {
                obj = o;
                isNode = false;
            }

            public PartitionNode<T> GetNode()
            {
                return node;
            }

            public SpatialObj<T> GetObj()
            {
                return obj;
            }

            public bool IsNode()
            {
                return isNode;
            }

            public bool IsObj()
            {
                return !isNode;
            }

            public bool Equals(NodeOrObj other)
            {
                if (other.IsNode() && IsNode() && other.GetNode().Equals(node))
                {
                    return true;
                }
                if (other.IsObj() && IsObj() && other.GetObj().Equals(obj))
                {
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals(obj as NodeOrObj);
            }

            public override int GetHashCode()
            {
                if (IsNode())
                {
                    return node.GetHashCode();
                }
                if (IsObj())
                {
                    return obj.GetHashCode();
                }
                return 0;
            }
        }


        private PartitionNode<T> rootNode;
        private Rectangle Bounds;
        private int Capacity;

        private SimplePriorityQueue<NodeOrObj> incrNN_queue;
        private Point incrNN_origin;

        public int Count { get; set; }

        public PartitionFieldTreeStructure(Rectangle bounds, int capacity)
        {
            Bounds = bounds;
            Capacity = capacity;
            rootNode = new PartitionNode<T>(bounds, capacity, 0, new List<PartitionNode<T>>());
        }

        #region Add and Remove

        public void AddManyRectangles(List<SpatialObj<T>> rects)
        {
            foreach (SpatialObj<T> rect in rects)
            {
                AddRectangle(rect, false);
            }
            ReorganizeOverflownNodes();
        }

        public PartitionNode<T> AddRectangle(SpatialObj<T> rect, bool reorganize = true)
        {
            PartitionNode<T> deepest_field = FindContainingField(rect, rootNode);

            if (deepest_field.IsFull() && !deepest_field.HasChildren())
            {
                PartitionField(deepest_field);
                PartitionNode<T> node = AddRectangle(rect);
                if (reorganize)
                    ReorganizeOverflownNodes();
                if (node != null)
                    return node;
            }
            else
            {
                bool overflown = deepest_field.StoreRectangle(rect);
            }
            if (deepest_field != null)
            {
                Count += 1;
            }
            return (deepest_field);
        }

        private void PartitionField(PartitionNode<T> node)
        {
            node.CreateChildren();
        }

        private void ReorganizeOverflownNodes()
        {
            ReorganizeNode(rootNode);
        }

        private void ReorganizeNode(PartitionNode<T> node)
        {
            // After adding objects and creating new partitions, checks to see if any of the upper level objects can go deeper into the tree.
            Queue<PartitionNode<T>> all_nodes = new Queue<PartitionNode<T>>();
            all_nodes.Enqueue(node);

            while (all_nodes.Count > 0)
            {
                List<SpatialObj<T>> rects_removed = new List<SpatialObj<T>>();
                PartitionNode<T> current_node = all_nodes.Dequeue();

                foreach (SpatialObj<T> rect in current_node.GetOverflowObjs())
                {
                    PartitionNode<T> deepest_field = FindContainingField(rect, current_node);

                    if (!deepest_field.Equals(current_node))
                    {
                        bool overflown = deepest_field.StoreRectangle(rect);
                        rects_removed.Add(rect);
                    }
                }
                if (rects_removed.Count > 0)
                {
                    current_node.DeleteRectangles(rects_removed, true);
                }

                if (current_node.HasChildren())
                {
                    foreach (PartitionNode<T> child in current_node.GetChildren())
                    {
                        all_nodes.Enqueue(child);
                    }
                }
            }
        }

        public PartitionNode<T> FindContainingField(SpatialObj<T> rect, PartitionNode<T> starting_node)
        {
            if (starting_node.IsPointInNode(rect.boundingBox.Center))
            {
                bool contained_in_node = starting_node.IsObjectInNode(rect);
                if (contained_in_node && !starting_node.CanSink(rect))
                {
                    return starting_node;
                }
                foreach (PartitionNode<T> child in starting_node.GetChildren())
                {
                    var potential_node = FindContainingField(rect, child);
                    if (potential_node != null)
                    {
                        return potential_node;
                    }
                }
                if (contained_in_node)
                {
                    return starting_node;
                }
            }
            return null;
        }

        public bool IsEmpty()
        {
            return (rootNode.IsEmpty() && !rootNode.HasChildren());
        }

        private SpatialObj<T> RemoveRectangle(SpatialObj<T> rect, PartitionNode<T> node)
        {
            if (node.DeleteRectangle(rect))
            {
                Count -= 1;
            }
            MergeEmptyChildren(node);
            return rect;
        }

        private void MergeEmptyChildren(PartitionNode<T> node)
        {
            if (node.HasChildren() && node.AreExistingChildrenEmpty())
            {
                node.MergeEmptyChildren();
            }
            if (node.IsRootNode() || !node.IsEmpty() || node.HasChildren())
            {
                return;
            }
            foreach (var parent in node.GetParent())
            {
                MergeEmptyChildren(parent);
            }
        }

        public SpatialObj<T> FindNearestObjAndRemove(Point p)
        {
            SpatialObj<T> obj = default(SpatialObj<T>);
            PartitionNode<T> node = null;
            foreach (KeyValuePair<PartitionNode<T>, SpatialObj<T>> entry in FindNearestRectToPoint(p, rootNode))
            {
                obj = entry.Value;
                node = entry.Key;
            }

            if (node != null)
            {
                return RemoveRectangle(obj, node);
            }
            return obj;
        }

        public SpatialObj<T> RemoveObject(SpatialObj<T> rect)
        {
            PartitionNode<T> node = null;
            foreach (KeyValuePair<PartitionNode<T>, SpatialObj<T>> entry in FindNearestRectToPoint(rect.boundingBox.Center, rootNode))
            {
                if (entry.Value.Equals(rect))
                {
                    node = entry.Key;
                }
            }

            if (node != null)
            {
                return RemoveRectangle(rect, node);
            }
            return rect;
        }

        #endregion

        #region Query

        public List<SpatialObj<T>> FindNearestObj(Point p)
        {
            List<SpatialObj<T>> results = new List<SpatialObj<T>>();
            foreach (KeyValuePair<PartitionNode<T>, SpatialObj<T>> entry in FindNearestRectToPoint(p, rootNode))
            {
                results.Add(entry.Value);
            }
            return results;
        }

        private Dictionary<PartitionNode<T>, SpatialObj<T>> FindNearestRectToPoint(Point p, PartitionNode<T> node)
        {
            Dictionary<PartitionNode<T>, SpatialObj<T>> answer_dict = new Dictionary<PartitionNode<T>, SpatialObj<T>>();
            SimplePriorityQueue<PartitionNode<T>> searching_nodes = new SimplePriorityQueue<PartitionNode<T>>();
            searching_nodes.Enqueue(node, 0);

            SpatialObj<T> answer = default(SpatialObj<T>);
            PartitionNode<T> answer_node = null;
            bool used = false;

            double min_distance_sq = rootNode.GetMaxDistance();

            while (searching_nodes.Count > 0)
            {
                PartitionNode<T> current_node = searching_nodes.Dequeue();
                Dictionary<SpatialObj<T>, double> nearest_rect = current_node.GetNearestRectangle(p);
                if (nearest_rect.Count > 0)
                {
                    foreach (KeyValuePair<SpatialObj<T>, double> entry in nearest_rect)
                    {
                        if (entry.Value <= min_distance_sq || entry.Value < Statics.EPSILON)
                        {
                            min_distance_sq = entry.Value;
                            answer = entry.Key;
                            answer_node = current_node;
                            used = false;
                            if (min_distance_sq < Statics.EPSILON)
                            {
                                answer_dict.Add(answer_node, answer);
                                used = true;
                            }
                        }
                    }
                }
                if (current_node.HasChildren())
                {
                    foreach (PartitionNode<T> child in current_node.GetChildren())
                    {
                        double field_dist = child.GetDistanceToPointSq(p);
                        if (field_dist <= min_distance_sq)
                            searching_nodes.Enqueue(child, (float)field_dist);
                    }
                }
            }
            if (!used)
            {
                answer_dict.Add(answer_node, answer);
            }
            return answer_dict;

        }

        public List<SpatialObj<T>> RangeQuery(Point center, int radius)
        {
            // Finds objects within a Circular range
            List<SpatialObj<T>> answer = new List<SpatialObj<T>>();
            Queue<PartitionNode<T>> searching_nodes = new Queue<PartitionNode<T>>();

            searching_nodes.Enqueue(rootNode);
            while (searching_nodes.Count > 0)
            {
                PartitionNode<T> current_node = searching_nodes.Dequeue();
                if (current_node.IntersectsWith(center, radius))
                {
                    answer.AddRange(current_node.GetRangeQueryObj(center, radius));
                    if (current_node.HasChildren())
                    {
                        foreach (PartitionNode<T> child in current_node.GetChildren())
                        {
                            if (!searching_nodes.Contains(child))
                                searching_nodes.Enqueue(child);
                        }
                    }
                }
            }
            return answer;
        }

        public List<SpatialObj<T>> WindowQuery(Rectangle window)
        {
            // Finds objects within a Rectangular range (window)
            List<SpatialObj<T>> answer = new List<SpatialObj<T>>();
            Queue<PartitionNode<T>> searching_nodes = new Queue<PartitionNode<T>>();

            searching_nodes.Enqueue(rootNode);
            while (searching_nodes.Count > 0)
            {
                PartitionNode<T> current_node = searching_nodes.Dequeue();
                if (current_node.IntersectsWith(window))
                {
                    answer.AddRange(current_node.GetRangeQueryObj(window));
                    if (current_node.HasChildren())
                    {
                        foreach (PartitionNode<T> child in current_node.GetChildren())
                        {
                            if (!searching_nodes.Contains(child))
                                searching_nodes.Enqueue(child);
                        }
                    }
                }
            }
            return answer;
        }

        #endregion


        #region Incremental Nearest Neighbor Search

        public void InitIncrementalNN(Point p)
        {
            incrNN_queue = new SimplePriorityQueue<NodeOrObj>();
            NodeOrObj root_nodeOrObj = new NodeOrObj();
            root_nodeOrObj.SetNode(rootNode);
            incrNN_queue.Enqueue(root_nodeOrObj, 0);
            incrNN_origin = p;
        }

        public Tuple<PartitionNode<T>, SpatialObj<T>> IncrementalNNFindNext()
        {
            if (incrNN_queue is null)
            {
                return null;
            }

            while (incrNN_queue.Count > 0)
            {
                NodeOrObj current_element = incrNN_queue.Dequeue();
                while (incrNN_queue.Count > 0 && incrNN_queue.First().Equals(current_element))
                {
                    incrNN_queue.Dequeue();
                }

                if (current_element.IsObj())
                {
                    return Tuple.Create(current_element.GetNode(), current_element.GetObj());
                }
                else
                {
                    PartitionNode<T> current_node = current_element.GetNode();
                    double current_dist = current_node.GetDistanceToPointSq(incrNN_origin);

                    if (!current_node.IsEmpty())
                    {
                        foreach (SpatialObj<T> obj in current_node.GetAllObjects())
                        {
                            double distance = obj.boundingBox.GetDistanceSqToPoint(incrNN_origin);
                            if (distance >= current_dist)
                            {
                                NodeOrObj obj_nodeOrObj = new NodeOrObj();
                                obj_nodeOrObj.SetNode(current_node);
                                obj_nodeOrObj.SetObj(obj);
                                incrNN_queue.Enqueue(obj_nodeOrObj, (float)distance);
                            }
                        }
                    }
                    if (current_node.HasChildren())
                    {
                        foreach (PartitionNode<T> child_node in current_node.GetChildren())
                        {
                            double distance = child_node.GetDistanceToPointSq(incrNN_origin);
                            if (distance >= current_dist)
                            {
                                NodeOrObj node_nodeOrObj = new NodeOrObj();
                                node_nodeOrObj.SetNode(child_node);
                                incrNN_queue.Enqueue(node_nodeOrObj, (float)distance);
                            }
                        }
                    }
                }
            }
            return null;
        }

        #endregion

    }
}
