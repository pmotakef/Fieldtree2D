using FieldTree2D_v2.Spatial;
using FieldTree2D_v2.Geometry;
using FieldTree2D_v2.FieldTree;

namespace FieldTree2D_v2
{
    public class PartitionFieldTree<T> where T : ISpatial
    {

        private PartitionFieldTreeStructure<T> fieldTree;
        private Size size;
        private Point Center;
        private int Capacity;

        /// <summary>
        /// Sets up the Partition Field Tree Data Structure
        /// </summary>
        /// <param name="width">Total width of data extent.</param>
        /// <param name="height">Total height of data extent.</param>
        /// <param name="centerX">X-coordinate of the center of the data structure. Defualts to 0.</param>
        /// <param name="centerY">Y-coordinate of the center of the data structure. Defualts to 0</param>
        /// <param name="capacity">Total capacity of each node. Used for partitioning. Defualts to 2 object per node.</param>
        public PartitionFieldTree(int width, int height, int centerX = 0, int centerY = 0, int capacity = 2)
        {
            size = new Size(width, height);
            Center = new Point(centerX, centerY);
            Capacity = capacity;
            Clear();
        }

        /// <summary>
        /// Creates a new Partition Field Tree and dumps the old one.
        /// </summary>
        public void Clear()
        {
            fieldTree = new PartitionFieldTreeStructure<T>(new Rectangle(Center, size), Capacity);
        }

        /// <summary>
        /// Tree is empty if no object is saved in it.
        /// </summary>
        /// <returns>True/False whether the tree is empty.</returns>
        public bool IsEmpty()
        {
            return fieldTree.IsEmpty();
        }

        /// <summary>
        /// Adds an object to the tree.
        /// </summary>
        /// <param name="obj">Spatial type object to be added to the tree. Must implement ISpatial interface.</param>
        public void Add(T obj)
        {
            fieldTree.AddRectangle(new SpatialObj<T>(obj));
        }

        /// <summary>
        /// Adds a collection of objects to the tree.
        /// </summary>
        /// <param name="objs">Collection of spatial type objects to be added to the tree. Each object must implement ISpatial interface.</param>
        public void AddMany(List<T> objs)
        {
            foreach (var obj in objs)
            {
                Add(obj);
            }
        }

        /// <summary>
        /// Finds and removes the given object from the tree.
        /// </summary>
        /// <param name="obj">Spatial object to be removed from the tree.</param>
        public void Remove(T obj)
        {
            fieldTree.RemoveObject(new SpatialObj<T>(obj));
        }

        /// <summary>
        /// Point-Query Pop: Finds the nearest spatial object the the given point, remove it from the tree, and returns the object.
        /// </summary>
        /// <param name="pointX">X-coordiante of the point-query.</param>
        /// <param name="pointY">Y-coordiante of the point-query.</param>
        /// <returns>The closest spatial object to the point-query coordinates.</returns>
        public T PopNearestObject(int pointX, int pointY)
        {
            Point p = new Point(pointX, pointY);
            return fieldTree.FindNearestObjAndRemove(p).objInstance;
        }

        /// <summary>
        /// Point-Query: Finds the nearest spatial object the the given point. 
        /// If the query-point intersects with multiple objects all intersecting objects are returned.
        /// Otherwise the return list contains a single query object.
        /// </summary>
        /// <param name="pointX">X-coordiante of the point-query.</param>
        /// <param name="pointY">Y-coordiante of the point-query.</param>
        /// <returns>A list of intersecting objects or the closest spatial object to the point-query coordinates.</returns>
        public List<T> FindNearestObjects(int pointX, int pointY)
        {
            Point p = new Point(pointX, pointY);
            return fieldTree.FindNearestObj(p).Select(x => x.objInstance).ToList();
        }

        /// <summary>
        /// Range Query: Finds all objects contained in a circular range defined by its center and its radius.
        /// </summary>
        /// <param name="centerX">X-coordiante of the range-query center.</param>
        /// <param name="centerY">Y-coordiante of the range-query center.</param>
        /// <param name="radius">Radius of the range-query.</param>
        /// <returns>A list of spatial objects contained in the range query.</returns>
        public List<T> RangeQuery(int centerX, int centerY, int radius)
        {
            Point center = new Point(centerX, centerY);
            return fieldTree.RangeQuery(center, radius).Select(x => x.objInstance).ToList();
        }

        /// <summary>
        /// Range Query: Finds all objects contained in a rectangular range defined by its center and its size.
        /// </summary>
        /// <param name="centerX">X-coordiante of the range-query center.</param>
        /// <param name="centerY">Y-coordiante of the range-query center.</param>
        /// <param name="width">Width of the range-query.</param>
        /// <param name="height">Height of the range-query.</param>
        /// <returns>A list of spatial objects contained in the range query.</returns>
        public List<T> RangeQuery(int centerX, int centerY, int width, int height)
        {
            Rectangle window = new Rectangle(new Point(centerX, centerY), width, height);
            return fieldTree.WindowQuery(window).Select(x => x.objInstance).ToList();
        }

        /// <summary>
        /// Prepares the data structure for incremental nearest neighbor query form the given point.
        /// </summary>
        /// <param name="pointX">X-coordiante of the incremental nearest neighbor point-query.</param>
        /// <param name="pointY">Y-coordiante of the incremental nearest neighbor point-query.</param>
        public void InitIncrementalNearestNeighbor(int pointX, int pointY)
        {
            fieldTree.InitIncrementalNN(new Point(pointX, pointY));
        }

        /// <summary>
        /// Finds the next item in the queue closest to the query point.
        /// </summary>
        /// <returns>Next closest spatial object to the query point.</returns>
        public T IncrementalNearestNeighborFindNext()
        {
            var nextObj = fieldTree.IncrementalNNFindNext();
            if (nextObj != null)
            {
                return nextObj.Item2.objInstance;
            }
            return default(T);
        }

        /// <summary>
        /// Gets all saved objects in the tree.
        /// </summary>
        /// <returns>All saved objects.</returns>
        public List<T> All()
        {
            return RangeQuery(Center.X, Center.Y, size.Width, size.Height);
        }

        /// <summary>
        /// Total number of objects saved in the structure
        /// </summary>
        /// <returns>Total number of objects</returns>
        public int Count()
        {
            return fieldTree.Count;
        }
    }
}
