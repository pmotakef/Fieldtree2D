using Microsoft.VisualStudio.TestTools.UnitTesting;
using FieldTree2D_v2.Spatial;
using FieldTree2D_v2.Geometry;
using FieldTree2D_v2;

namespace FieldTreeUnitTests
{
    public class Rect : ISpatial
    {
        public int CenterX { get; set; }
        public int CenterY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool Equals(ISpatial other)
        {
            return (CenterX == other.CenterX) && (CenterY == other.CenterY) && (Width == other.Width) && (Height == other.Height);
        }

        public Rect(Point center, Size size)
        {
            CenterX = center.X;
            CenterY = center.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public Rect()
        {
            CenterX = 0;
            CenterY = 0;
            Width = 1;
            Height = 1;
        }
    }

    [TestClass]
    public class CoverFieldTreeTest
    {
        [TestMethod]
        public void AddSingleRectangle()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            coverFieldTree.Add(rect1);
            Assert.AreEqual(coverFieldTree.IsEmpty(), false);
        }

        [TestMethod]
        public void AddSingleRectangleAndPop()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            coverFieldTree.Add(rect1);
            Rect r = coverFieldTree.PopNearestObject(0, 0);

            Assert.AreEqual(coverFieldTree.IsEmpty(), true);
            Assert.AreEqual(r, rect1);
        }

        [TestMethod]
        public void AddSingleRectangleAndRemove()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            coverFieldTree.Add(rect1);
            coverFieldTree.Remove(rect1);
            Assert.AreEqual(coverFieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestClear()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            Rect rect2 = new Rect(new Point(-2, 2), new Size(1, 1));
            Rect rect3 = new Rect(new Point(3, 2), new Size(1, 2));
            Rect rect4 = new Rect(new Point(2, -2), new Size(1, 1));
            coverFieldTree.Add(rect1);
            coverFieldTree.Add(rect2);
            coverFieldTree.Add(rect3);
            coverFieldTree.Add(rect4);
            coverFieldTree.Clear();
            Assert.AreEqual(coverFieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestAddMultipleRectangles()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            coverFieldTree.AddMany(rects);
            Assert.AreEqual(coverFieldTree.IsEmpty(), false);
            var r = coverFieldTree.FindNearestObjects(0, 0);
            Assert.AreEqual(r.Count, 1);
            Assert.AreEqual(r[0].Equals(new Rect(new Point(-1, 0), new Size(1, 1))), true);

            r = coverFieldTree.FindNearestObjects(2, 0);
            Assert.AreEqual(r.Count, 1);
            Assert.AreEqual(r[0].Equals(new Rect(new Point(1, 1), new Size(1, 1))), true);
        }

        [TestMethod]
        public void TestRangeQuery()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            coverFieldTree.AddMany(rects);
            Assert.AreEqual(coverFieldTree.IsEmpty(), false);

            var r1 = coverFieldTree.RangeQuery(0, 0, 3);
            Assert.AreEqual(r1.Count, 2);

            var r2 = coverFieldTree.RangeQuery(0, 0, 3, 2);
            Assert.AreEqual(r2.Count, 1);

            var r3 = coverFieldTree.RangeQuery(-1, 0, 4, 10);
            Assert.AreEqual(r3.Count, 2);
        }

        [TestMethod]
        public void TestINN()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(10, 10, 0.3, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            coverFieldTree.AddMany(rects);
            Assert.AreEqual(coverFieldTree.IsEmpty(), false);

            coverFieldTree.InitIncrementalNearestNeighbor(0, 0);
            var r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(-1, 0), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(1, 1), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(2, -2), new Size(1, 1))) || r.Equals(new Rect(new Point(-2, -2), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(2, -2), new Size(1, 1))) || r.Equals(new Rect(new Point(-2, -2), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(-3, 2), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(3, 3), new Size(1, 1))) || r.Equals(new Rect(new Point(3, -3), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(3, 3), new Size(1, 1))) || r.Equals(new Rect(new Point(3, -3), new Size(1, 1))), true);
            r = coverFieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r, null);
        }

        [TestMethod]
        public void TestAll()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(16, 16, 0.3, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            coverFieldTree.AddMany(rects);
            Assert.AreEqual(coverFieldTree.IsEmpty(), false);

            var r1 = coverFieldTree.All();
            Assert.AreEqual(r1.Count, 7);
        }

        [TestMethod]
        public void TestRemoveMany()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(16, 16, 0.3, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            coverFieldTree.AddMany(rects);
            Assert.AreEqual(coverFieldTree.IsEmpty(), false);

            foreach (var item in rects)
            {
                coverFieldTree.Remove(item);
            }
            Assert.AreEqual(coverFieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestCount()
        {
            CoverFieldTree<Rect> coverFieldTree = new CoverFieldTree<Rect>(16, 16, 0.3, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            coverFieldTree.AddMany(rects);
            Assert.AreEqual(coverFieldTree.Count(), 7);

            int totalNum = 7;

            foreach (var item in rects)
            {
                coverFieldTree.Remove(item);
                totalNum -= 1;
                Assert.AreEqual(coverFieldTree.Count(), totalNum);
            }
            Assert.AreEqual(coverFieldTree.IsEmpty(), true);
        }

    }


    [TestClass]
    public class PartitionFieldTreeTest
    {
        [TestMethod]
        public void AddSingleRectangle()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(10, 10);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            fieldTree.Add(rect1);
            Assert.AreEqual(fieldTree.IsEmpty(), false);
        }

        [TestMethod]
        public void AddSingleRectangleAndPop()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(10, 10);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            fieldTree.Add(rect1);
            Rect r = fieldTree.PopNearestObject(0, 0);

            Assert.AreEqual(fieldTree.IsEmpty(), true);
            Assert.AreEqual(r, rect1);
        }

        [TestMethod]
        public void AddSingleRectangleAndRemove()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(10, 10);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            fieldTree.Add(rect1);
            fieldTree.Remove(rect1);
            Assert.AreEqual(fieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestClear()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(10, 10);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            Rect rect2 = new Rect(new Point(-2, 2), new Size(1, 1));
            Rect rect3 = new Rect(new Point(3, 2), new Size(1, 2));
            Rect rect4 = new Rect(new Point(2, -2), new Size(1, 1));
            fieldTree.Add(rect1);
            fieldTree.Add(rect2);
            fieldTree.Add(rect3);
            fieldTree.Add(rect4);
            fieldTree.Clear();
            Assert.AreEqual(fieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestAddMultipleRectangles()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(10, 10);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            fieldTree.AddMany(rects);
            Assert.AreEqual(fieldTree.IsEmpty(), false);
            var r = fieldTree.FindNearestObjects(0, 0);
            Assert.AreEqual(r.Count, 1);
            Assert.AreEqual(r[0].Equals(new Rect(new Point(-1, 0), new Size(1, 1))), true);

            r = fieldTree.FindNearestObjects(2, 0);
            Assert.AreEqual(r.Count, 1);
            Assert.AreEqual(r[0].Equals(new Rect(new Point(1, 1), new Size(1, 1))), true);
        }

        [TestMethod]
        public void TestRangeQuery()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(8, 8, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            fieldTree.AddMany(rects);
            Assert.AreEqual(fieldTree.IsEmpty(), false);

            var r1 = fieldTree.RangeQuery(0, 0, 3);
            Assert.AreEqual(r1.Count, 2);

            var r2 = fieldTree.RangeQuery(0, 0, 3, 2);
            Assert.AreEqual(r2.Count, 1);

            var r3 = fieldTree.RangeQuery(-1, 0, 4, 10);
            Assert.AreEqual(r3.Count, 2);
        }

        [TestMethod]
        public void TestINN()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(10, 10, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            fieldTree.AddMany(rects);
            Assert.AreEqual(fieldTree.IsEmpty(), false);

            fieldTree.InitIncrementalNearestNeighbor(0, 0);
            var r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(-1, 0), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(1, 1), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(2, -2), new Size(1, 1))) || r.Equals(new Rect(new Point(-2, -2), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(2, -2), new Size(1, 1))) || r.Equals(new Rect(new Point(-2, -2), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(-3, 2), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(3, 3), new Size(1, 1))) || r.Equals(new Rect(new Point(3, -3), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r.Equals(new Rect(new Point(3, 3), new Size(1, 1))) || r.Equals(new Rect(new Point(3, -3), new Size(1, 1))), true);
            r = fieldTree.IncrementalNearestNeighborFindNext();
            Assert.AreEqual(r, null);
        }

        [TestMethod]
        public void TestAll()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(16, 16, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            fieldTree.AddMany(rects);
            Assert.AreEqual(fieldTree.IsEmpty(), false);

            var r1 = fieldTree.All();
            Assert.AreEqual(r1.Count, 7);
        }

        [TestMethod]
        public void TestRemoveMany()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(16, 16, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            fieldTree.AddMany(rects);
            Assert.AreEqual(fieldTree.IsEmpty(), false);

            foreach (var item in rects)
            {
                fieldTree.Remove(item);
            }
            Assert.AreEqual(fieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestCount()
        {
            PartitionFieldTree<Rect> fieldTree = new PartitionFieldTree<Rect>(16, 16, 0, 0, 1);
            List<Rect> rects = new List<Rect>()
            {
                new Rect(new Point(1, 1), new Size(1, 1)),
                new Rect(new Point(3, 3), new Size(1, 1)),
                new Rect(new Point(-3, 2), new Size(1, 1)),
                new Rect(new Point(-1, 0), new Size(1, 1)),
                new Rect(new Point(-2, -2), new Size(1, 1)),
                new Rect(new Point(2, -2), new Size(1, 1)),
                new Rect(new Point(3, -3), new Size(1, 1))
            };
            fieldTree.AddMany(rects);
            Assert.AreEqual(fieldTree.Count(), 7);

            int totalNum = 7;

            foreach (var item in rects)
            {
                fieldTree.Remove(item);
                totalNum -= 1;
                Assert.AreEqual(fieldTree.Count(), totalNum);
            }
            Assert.AreEqual(fieldTree.IsEmpty(), true);
        }
    }


    [TestClass]
    public class RectangleTest
    {
        [TestMethod]
        public void PointInRectangleTest()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(4, 10));
            Assert.AreEqual(rect.ContainsPoint(new Point(6, 6)), true);
            Assert.AreEqual(rect.ContainsPoint(new Point(5, 5)), true);
            Assert.AreEqual(rect.ContainsPoint(new Point(7, 9)), true);
            Assert.AreEqual(rect.ContainsPoint(new Point(8, 6)), false);
            Assert.AreEqual(rect.ContainsPoint(new Point(1, 2)), false);
        }

        [TestMethod]
        public void RectangleContainsRectTest()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(4, 10));
            Assert.AreEqual(rect.ContainsRect(new Rectangle(new Point(5, 5), new Size(2, 4))), true);
            Assert.AreEqual(rect.ContainsRect(new Rectangle(new Point(5, 5), new Size(6, 4))), false);
            Assert.AreEqual(rect.ContainsRect(new Rectangle(new Point(6, 6), new Size(2, 2))), true);
        }

        [TestMethod]
        public void RectangleContainedByRectTest()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(4, 10));
            Assert.AreEqual(rect.ContainedByRect(new Rectangle(new Point(5, 5), new Size(2, 4))), false);
            Assert.AreEqual(rect.ContainedByRect(new Rectangle(new Point(6, 6), new Size(10, 20))), true);
        }

        [TestMethod]
        public void RectangleContainedByCircleTest()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(4, 10));
            Assert.AreEqual(rect.ContainedByCircle(new Point(5, 5), 20), true);
            Assert.AreEqual(rect.ContainedByCircle(new Point(5, 5), 4), false);
        }

        [TestMethod]
        public void RectangleIntersectsCircle()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(4, 10));
            Assert.AreEqual(rect.IntersectsCircle(new Point(5, 5), 20), true);
            Assert.AreEqual(rect.IntersectsCircle(new Point(5, 5), 4), true);
            Assert.AreEqual(rect.IntersectsCircle(new Point(9, 6), 4), true);
        }

        [TestMethod]
        public void RectangleIntersectsRect()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(4, 10));
            Assert.AreEqual(rect.IntersectsWith(new Rectangle(new Point(5, 5), new Size(2, 4))), true);
            Assert.AreEqual(rect.IntersectsWith(new Rectangle(new Point(6, 6), new Size(10, 20))), true);
            Assert.AreEqual(rect.IntersectsWith(new Rectangle(new Point(0, 6), new Size(10, 5))), true);
            Assert.AreEqual(rect.IntersectsWith(new Rectangle(new Point(0, 6), new Size(2, 2))), false);
        }

        [TestMethod]
        public void RectangleToString()
        {
            Rectangle rect = new Rectangle(new Point(5, 5), new Size(1, 1));
            Assert.AreEqual(rect.ToString(), "(4.5, 4.5) - (5.5, 5.5)");
            rect = new Rectangle(new Point(5, 5), new Size(2, 2));
            Assert.AreEqual(rect.ToString(), "(4, 4) - (6, 6)");
        }
    }
}
