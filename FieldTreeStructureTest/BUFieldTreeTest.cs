using Microsoft.VisualStudio.TestTools.UnitTesting;
using FieldTreeStructure;
using FieldTreeStructure.Geometry;
using FieldTreeStructure.Spatial;
using System.Collections.Generic;

namespace FieldTreeStructureTest
{
    [TestClass]
    public class BUFieldTreeTest
    {
        [TestMethod]
        public void AddSingleRectangle()
        {
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            fieldTree.Add(rect1);
            Assert.AreEqual(fieldTree.IsEmpty(), false);
        }

        [TestMethod]
        public void AddSingleRectangleAndPop()
        {
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            fieldTree.Add(rect1);
            Rect r = fieldTree.PopNearestObject(0, 0);

            Assert.AreEqual(fieldTree.IsEmpty(), true);
            Assert.AreEqual(r, rect1);
        }

        [TestMethod]
        public void AddSingleRectangleAndRemove()
        {
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3);

            Rect rect1 = new Rect(new Point(2, 2), new Size(3, 3));
            fieldTree.Add(rect1);
            fieldTree.Remove(rect1);
            Assert.AreEqual(fieldTree.IsEmpty(), true);
        }

        [TestMethod]
        public void TestClear()
        {
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3);

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
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3);
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
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3, 1);
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
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3, 1);
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
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3, 1);
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
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3, 1);
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
            ButtomUpCoverFieldTree<Rect> fieldTree = new ButtomUpCoverFieldTree<Rect>(0.3, 1);
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
}
