﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq; 
using Geometry;
using System.Diagnostics;
using System.Collections.Generic;

namespace GeometryTests
{
    [TestClass]
    public class GridPolygonTest
    {
        /// <summary>
        /// Create a box, note I've added an extra vertex on the X:-1 vertical line
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        GridVector2[] BoxVerticies(double scale)
        {
            GridVector2[] ExteriorPoints =
            {
                new GridVector2(-1, -1),
                new GridVector2(-1, 0),
                new GridVector2(-1, 1),
                new GridVector2(1,1),
                new GridVector2(1,-1),
                new GridVector2(-1,-1)
            };

            GridVector2[] ExteriorPointsScaled = ExteriorPoints.Scale(scale, new GridVector2(0, 0)).ToArray();
            return ExteriorPointsScaled;
        }


        GridVector2[] ConcaveUVerticies(double scale)
        {
            //  *--*    *--*
            //  |  |    |  |
            //  |  |    |  |  
            //  |  *----*  |
            //  *----------*
            GridVector2[] ExteriorPoints =
            {
                new GridVector2(-1, -1),
                new GridVector2(-1, 1),
                new GridVector2(-0.5, 1),
                new GridVector2(-0.5, -0.5),
                new GridVector2(0.5,-0.5),
                new GridVector2(0.5,1),
                new GridVector2(1,1),
                new GridVector2(1,-1),
                new GridVector2(-1,-1)
            };

            GridVector2[] ExteriorPointsScaled = ExteriorPoints.Scale(scale, new GridVector2(0, 0)).ToArray();
            return ExteriorPointsScaled;
        }

        GridPolygon CreateBoxPolygon(double scale)
        {
            GridVector2[] ExteriorPointsScaled = BoxVerticies(scale);

            return new GridPolygon(ExteriorPointsScaled);
        }

        GridPolygon CreateTrianglePolygon(double scale)
        {
            GridVector2[] ExteriorPoints =
            {
                new GridVector2(-1, -1),
                new GridVector2(-1, 1),
                new GridVector2(1, -1),
                new GridVector2(-1,-1)
            };

            return new GridPolygon(ExteriorPoints);

        }

        GridPolygon CreateUPolygon(double scale)
        {
            GridVector2[] ExteriorPointsScaled = ConcaveUVerticies(scale);

            return new GridPolygon(ExteriorPointsScaled);
        }

        /// <summary>
        /// Ensure our Clockwise function works and that polygons are created Counter-Clockwise
        /// </summary>
        [TestMethod]
        public void ClockwiseTest()
        {
            GridVector2[] clockwisePoints = BoxVerticies(1);
            Assert.IsTrue(clockwisePoints.AreClockwise());

            GridVector2[] counterClockwisePoints = clockwisePoints.Reverse().ToArray();

            Assert.IsTrue(clockwisePoints[1] == counterClockwisePoints[counterClockwisePoints.Length - 2]);

            Assert.IsFalse(counterClockwisePoints.AreClockwise());

            GridPolygon clockwisePoly = new GridPolygon(clockwisePoints);
            GridPolygon counterClockwisePoly = new GridPolygon(clockwisePoints);

            Assert.IsFalse(clockwisePoly.ExteriorRing.AreClockwise());
            Assert.IsFalse(counterClockwisePoly.ExteriorRing.AreClockwise());
        }

        [TestMethod]
        public void AreaTest()
        {
            GridPolygon box = CreateBoxPolygon(10);
            Assert.AreEqual(box.Area, box.BoundingBox.Area);
            Assert.AreEqual(box.Area, 400);

            GridPolygon translated_box = box.Translate(new GridVector2(10, 10));
            Assert.AreEqual(Math.Round(translated_box.Area), translated_box.BoundingBox.Area);
            Assert.AreEqual(Math.Round(translated_box.Area), 400);
            Assert.AreEqual(Math.Round(translated_box.Area), box.Area);

            GridPolygon tri = CreateTrianglePolygon(10);
            Assert.AreEqual(tri.Area, tri.BoundingBox.Area / 2.0);
            Assert.AreEqual(tri.Area, 2);

            GridPolygon translated_tri = tri.Translate(new GridVector2(10, -10));
            Assert.AreEqual(translated_tri.Area, translated_tri.BoundingBox.Area / 2.0);
            Assert.AreEqual(translated_tri.Area, 2);
            Assert.AreEqual(translated_tri.Area, tri.Area);
        }

        [TestMethod]
        public void CentroidTest()
        {
            GridPolygon box = CreateBoxPolygon(10);
            Assert.AreEqual(box.Centroid, box.BoundingBox.Center);
        }

        [TestMethod]
        public void PolygonConvexContainsTest()
        {
            GridPolygon box = CreateBoxPolygon(10);
            Assert.IsFalse(box.Contains(new GridVector2(-15, 5)));
            Assert.IsTrue(box.Contains(new GridVector2(-5, 5)));
            Assert.IsTrue(box.Contains(new GridVector2(0, 0)));
            Assert.IsTrue(box.Contains(new GridVector2(-10, 0))); //Point exactly on the line
            Assert.IsTrue(box.Contains(new GridVector2(10, 0))); //Point exactly on the line
            Assert.IsTrue(box.Contains(new GridVector2(0, 10))); //Point exactly on the line
            Assert.IsTrue(box.Contains(new GridVector2(0, -10))); //Point exactly on the line

            GridPolygon inner_box = CreateBoxPolygon(5);
            Assert.IsTrue(box.Contains(inner_box));

            //OK, add an inner ring and make sure contains works
            box.AddInteriorRing(inner_box.ExteriorRing);

            Assert.IsFalse(box.Contains(new GridVector2(-15, 5)));
            Assert.IsFalse(box.Contains(new GridVector2(0, 0)));

            //Test points exactly on the inner ring
            Assert.IsTrue(box.Contains(new GridVector2(-5, 0)));
            Assert.IsTrue(box.Contains(new GridVector2(5, 0)));
            Assert.IsTrue(box.Contains(new GridVector2(0, -5)));
            Assert.IsTrue(box.Contains(new GridVector2(0, 5)));


        }

        [TestMethod]
        public void PolygonConcaveContainsTest()
        {
            GridPolygon box = CreateUPolygon(10);
            Assert.IsFalse(box.Contains(new GridVector2(-15, 5)));
            Assert.IsTrue(box.Contains(new GridVector2(-6.6, -6.6)));
            Assert.IsFalse(box.Contains(new GridVector2(0, 0)));
            Assert.IsTrue(box.Contains(box.ExteriorRing.First()));

            GridPolygon outside = CreateUPolygon(1);
            Assert.IsFalse(box.Contains(outside));

            GridPolygon inside = outside.Translate(new GridVector2(0, -7.5));
            Assert.IsTrue(box.Contains(inside));
        }

        [TestMethod]
        public void PolygonTestLineIntersection()
        {
            GridPolygon OuterBox = CreateBoxPolygon(15);
            GridPolygon U = CreateUPolygon(10);
            OuterBox.AddInteriorRing(U);

            //Line entirely outside outer polygon
            GridLineSegment line = new GridLineSegment(new GridVector2(-16, -16), new GridVector2(16, -16));
            Assert.IsFalse(OuterBox.Intersects(line));
            
            //Line entirely inside polygon
            line = new GridLineSegment(new GridVector2(-14, -14), new GridVector2(14, 14));
            Assert.IsTrue(OuterBox.Intersects(line));

            //Line falls exactly over outside polygon segment
            line = new GridLineSegment(new GridVector2(-14, -15), new GridVector2(14, -15));
            Assert.IsTrue(OuterBox.Intersects(line));
            Assert.IsTrue(line.Intersects(OuterBox, false));
            Assert.IsFalse(line.Intersects(OuterBox, true));

            //Line falls exactly over inner polygon segment
            line = new GridLineSegment(new GridVector2(-10, -10), new GridVector2(10, -10));
            Assert.IsTrue(OuterBox.Intersects(line));
            Assert.IsTrue(line.Intersects(OuterBox, false));
            Assert.IsFalse(line.Intersects(OuterBox, true));

            //Line inside inner polygon
            line = new GridLineSegment(new GridVector2(-7.5, -7.5), new GridVector2(7.5, -7.5));
            Assert.IsFalse(OuterBox.Intersects(line));
            Assert.IsFalse(line.Intersects(OuterBox));

            //Line is outside the polygon, but touches a vertex
            line = new GridLineSegment(new GridVector2(-20, -15), new GridVector2(-15, -15));
            Assert.IsTrue(OuterBox.Intersects(line));
            Assert.IsTrue(line.Intersects(OuterBox));
            Assert.IsFalse(line.Intersects(OuterBox, true));

            //Line inside inner polygon but touches a vertex
            line = new GridLineSegment(new GridVector2(-10, -10), new GridVector2(-7.5, -7.5));
            Assert.IsTrue(OuterBox.Intersects(line));
            Assert.IsTrue(line.Intersects(OuterBox));
            Assert.IsFalse(line.Intersects(OuterBox, true));
        }

        [TestMethod]
        public void PolygonTestLineCrossesPolygon()
        {
            GridPolygon OuterBox = CreateBoxPolygon(15);
            GridPolygon U = CreateUPolygon(10);
            OuterBox.AddInteriorRing(U);

            //Line entirely outside outer polygon
            GridLineSegment line = new GridLineSegment(new GridVector2(-16, -16), new GridVector2(16, -16));
            Assert.IsFalse(line.Crosses(OuterBox));

            //Line entirely inside polygon
            line = new GridLineSegment(new GridVector2(-14, -14), new GridVector2(14, 14));
            Assert.IsTrue(line.Crosses(OuterBox));

            //Line falls exactly over outside polygon segment
            line = new GridLineSegment(new GridVector2(-14, -15), new GridVector2(14, -15));
            Assert.IsFalse(line.Crosses(OuterBox));

            //Line falls exactly over inner polygon segment
            line = new GridLineSegment(new GridVector2(-10, -10), new GridVector2(10, -10));
            Assert.IsFalse(line.Crosses(OuterBox));

            //Line falls exactly over part of the inner polygon segment, then enters the polygon
            line = new GridLineSegment(new GridVector2(-12.5, -10), new GridVector2(10, -10));
            Assert.IsTrue(line.Crosses(OuterBox));

            //Line inside inner polygon
            line = new GridLineSegment(new GridVector2(-7.5, -7.5), new GridVector2(7.5, -7.5));
            Assert.IsFalse(line.Crosses(OuterBox));

            //Line is outside the polygon, but touches a vertex
            line = new GridLineSegment(new GridVector2(-20, -15), new GridVector2(-15, -15));
            Assert.IsFalse(line.Crosses(OuterBox));  

            //Line inside inner polygon but touches a vertex
            line = new GridLineSegment(new GridVector2(-10, -10), new GridVector2(-7.5, -7.5));
            Assert.IsFalse(line.Crosses(OuterBox));

            //Line touches two segments of the exterior ring
            line = new GridLineSegment(new GridVector2(-15, -14), new GridVector2(15, -14));
            Assert.IsTrue(line.Crosses(OuterBox));
        }


        [TestMethod]
        public void PolygonAddRemoveVertexTest()
        {
            GridPolygon original_box = CreateBoxPolygon(10);
            GridPolygon box = CreateBoxPolygon(10);
            int numOriginalVerticies = box.ExteriorRing.Length;
            GridVector2 newVertex = new GridVector2(-10, -5);
            box.AddVertex(newVertex);
            Assert.AreEqual(box.ExteriorRing.Length, numOriginalVerticies + 1);
            Assert.AreEqual(box.ExteriorRing[box.ExteriorRing.Length - 2], newVertex);

            box.RemoveVertex(newVertex);
            Assert.AreEqual(box.ExteriorRing.Length, numOriginalVerticies);

            box = CreateBoxPolygon(10);
            newVertex = new GridVector2(-5, -10);
            box.AddVertex(newVertex);
            Assert.AreEqual(box.ExteriorRing.Length, numOriginalVerticies + 1);
            Assert.AreEqual(box.ExteriorRing[1], newVertex);

            box.RemoveVertex(newVertex - new GridVector2(1, 1));
            Assert.AreEqual(box.ExteriorRing.Length, numOriginalVerticies);
            Assert.IsTrue(box.ExteriorRing.All(p => p != newVertex));
        }

        [TestMethod]
        public void PolygonAddPointsAtIntersectionsTest()
        {
            GridPolygon box = CreateBoxPolygon(10);
            GridPolygon U = CreateUPolygon(10);

            //Move the box so the top line is along Y=0 
            box = box.Translate(new GridVector2(0, -10));

            //This should add four verticies
            int OriginalVertCount = U.ExteriorRing.Length;
            U.AddPointsAtIntersections(box);

            Assert.IsTrue(OriginalVertCount + 4 == U.ExteriorRing.Length);
            Assert.IsTrue(U.ExteriorRing.Contains(new GridVector2(-10, 0)));
            Assert.IsTrue(U.ExteriorRing.Contains(new GridVector2(10, 0)));
            Assert.IsTrue(U.ExteriorRing.Contains(new GridVector2(-5, 0)));
            Assert.IsTrue(U.ExteriorRing.Contains(new GridVector2(5, 0)));
        }

        [TestMethod]
        public void PolygonAddPointsAtIntersectionsTest2()
        {
            GridPolygon box = CreateBoxPolygon(10);
            GridPolygon OuterBox = CreateBoxPolygon(15);
            GridPolygon U = CreateUPolygon(10);

            //Add the U polygon as an interior polygon
            OuterBox.AddInteriorRing(U);

            //Move the box so the top line is along Y=0 
            box = box.Translate(new GridVector2(0, -10));

            //This should add four verticies
            int OriginalExteriorVertCount = OuterBox.ExteriorRing.Length;
            int OriginalInnerVertCount = U.ExteriorRing.Length;
            OuterBox.AddPointsAtIntersections(box);

            GridPolygon NewU = OuterBox.InteriorPolygons.First();

            //Check that the interior ring was correctly appended
            Assert.IsTrue(OriginalInnerVertCount + 4 == NewU.ExteriorRing.Length);
            Assert.IsTrue(NewU.ExteriorRing.Contains(new GridVector2(-10, 0)));
            Assert.IsTrue(NewU.ExteriorRing.Contains(new GridVector2(10, 0)));
            Assert.IsTrue(NewU.ExteriorRing.Contains(new GridVector2(-5, 0)));
            Assert.IsTrue(NewU.ExteriorRing.Contains(new GridVector2(5, 0)));

            //Check that the exterior ring was correctly appended
            Assert.IsTrue(OriginalExteriorVertCount + 2 == OuterBox.ExteriorRing.Length);
            Assert.IsTrue(OuterBox.ExteriorRing.Contains(new GridVector2(-10, -15)));
            Assert.IsTrue(OuterBox.ExteriorRing.Contains(new GridVector2(10, -15)));

            //OK, now test from the other direction 

            OriginalExteriorVertCount = box.ExteriorRing.Length;
            box.AddPointsAtIntersections(OuterBox);

            //We should add 5 new verticies since the box had an extra vertex at -1,0 originally.  See CreateBoxPolygon
            Assert.IsTrue(OriginalExteriorVertCount + 5 == box.ExteriorRing.Length);
            Assert.IsTrue(box.ExteriorRing.Contains(new GridVector2(-10, -15)));
            Assert.IsTrue(box.ExteriorRing.Contains(new GridVector2(10, -15)));
            Assert.IsTrue(box.ExteriorRing.Contains(new GridVector2(-10, -10)));
            Assert.IsTrue(box.ExteriorRing.Contains(new GridVector2(-5, 0)));
            Assert.IsTrue(box.ExteriorRing.Contains(new GridVector2(5, 0)));
            Assert.IsTrue(box.ExteriorRing.Contains(new GridVector2(10, -10)));
        }

        [TestMethod]
        public void EnumeratePolygonIndiciesTest()
        {
            GridPolygon box = CreateBoxPolygon(10);
            GridPolygon OuterBox = CreateBoxPolygon(15);
            GridPolygon U = CreateUPolygon(10);
            GridPolygon U2 = CreateBoxPolygon(1);

            //Move the box so it doesn't overlap
            box = box.Translate(new GridVector2(50, 0));

            //Check a single polygon with no interior verticies
            GridPolygon[] polyArray = new GridPolygon[] { box };
            PolySetVertexEnum enumerator = new PolySetVertexEnum(polyArray);

            PointIndex[] indicies = enumerator.ToArray();
            Assert.IsTrue(indicies.Length == box.ExteriorRing.Length-1);
            Assert.IsTrue(indicies.Last().IsLastIndexInRing());
            Assert.IsTrue(indicies.Select(p => p.Point(polyArray)).Distinct().Count() == box.ExteriorRing.Length - 1); //Make sure all indicies are unique and not repeating

            for(int i = 0; i < indicies.Length; i++)
            {
                Assert.AreEqual(i, indicies[i].iVertex);
            }

            //Check a polygon with interior polygon
            OuterBox.AddInteriorRing(U);

            polyArray = new GridPolygon[] { OuterBox };
            enumerator = new PolySetVertexEnum(polyArray);
            indicies = enumerator.ToArray();
            int numUniqueVerticies = (OuterBox.ExteriorRing.Length - 1) + OuterBox.InteriorPolygons.Sum(ip => ip.ExteriorRing.Length - 1);
            Assert.IsTrue(indicies.Length == numUniqueVerticies);
            Assert.IsTrue(indicies.Select(p => p.Point(polyArray)).Distinct().Count() == numUniqueVerticies); //Make sure all indicies are unique and not repeating

            //Check a polygon with two interior polygon
            OuterBox.AddInteriorRing(U2);

            polyArray = new GridPolygon[] { OuterBox };
            enumerator = new PolySetVertexEnum(polyArray);
            indicies = enumerator.ToArray();
            numUniqueVerticies = (OuterBox.ExteriorRing.Length - 1) + OuterBox.InteriorPolygons.Sum(ip => ip.ExteriorRing.Length - 1);
            Assert.IsTrue(indicies.Length == numUniqueVerticies);
            Assert.IsTrue(indicies.Select(p => p.Point(polyArray)).Distinct().Count() == numUniqueVerticies); //Make sure all indicies are unique and not repeating

            //Check a polygon with two interior polygons and two polygons in the array

            polyArray = new GridPolygon[] { OuterBox, box };
            enumerator = new PolySetVertexEnum(polyArray);
            indicies = enumerator.ToArray();
            numUniqueVerticies = (box.ExteriorRing.Length -1) + (OuterBox.ExteriorRing.Length - 1) + OuterBox.InteriorPolygons.Sum(ip => ip.ExteriorRing.Length - 1);
            Assert.IsTrue(indicies.Length == numUniqueVerticies);
            Assert.IsTrue(indicies.Select(p => p.Point(polyArray)).Distinct().Count() == numUniqueVerticies); //Make sure all indicies are unique and not repeating
        }

        [TestMethod]
        public void SortPointIndexTest1()
        {
            //Test sorting when we need to prevent breaks at the wraparound at the 0 index..

            //Create an array where the first and last index are adjacent, but there is a gap in the center
            PointIndex[] points = new PointIndex[] {new PointIndex(0,0,6),
                                                    new PointIndex(0,1,6),
                                                    new PointIndex(0,2,6),
                                                    new PointIndex(0,4,6),
                                                    new PointIndex(0,5,6)};
            PointIndex[] sorted = PointIndex.SortByRing(points);

            Assert.IsTrue(sorted.First().iVertex == 4);
            Assert.IsTrue(sorted[1].iVertex == 5);
            Assert.IsTrue(sorted.Last().iVertex == 2);
        }

        [TestMethod]
        public void SortPointIndexTest2()
        {
            //Test sorting when we need to prevent breaks at the wraparound at the 0 index..

            //Create an array where the first and last index are adjacent, but there is a gap in the center
            PointIndex[] points = new PointIndex[] {new PointIndex(0,0,8),
                                                    new PointIndex(0,1,8),
                                                    new PointIndex(0,2,8),
                                                    new PointIndex(0,4,8),
                                                    new PointIndex(0,5,8),
                                                    new PointIndex(0,7,8)};
            PointIndex[] sorted = PointIndex.SortByRing(points);

            Assert.IsTrue(sorted.First().iVertex == 4);
            Assert.IsTrue(sorted[1].iVertex == 5);
            Assert.IsTrue(sorted[2].iVertex == 7);
            Assert.IsTrue(sorted.Last().iVertex == 2);
        }

        [TestMethod]
        public void SortPointIndexTest3()
        {
            //Test sorting when we need to prevent breaks at the wraparound at the 0 index..

            //Create an array where the first and last index are adjacent, but there is a gap in the center
            PointIndex[] points = new PointIndex[] {new PointIndex(0,0,8),
                                                    new PointIndex(0,1,8),
                                                    new PointIndex(0,2,8),
                                                    new PointIndex(0,4,8),
                                                    new PointIndex(0,5,8),
                                                    new PointIndex(0,7,8),

                                                    new PointIndex(0, 1, 0,8),
                                                    new PointIndex(0, 1, 1,8),
                                                    new PointIndex(0,1,2,8),
                                                    new PointIndex(0,1,4,8),
                                                    new PointIndex(0,1,5,8),
                                                    new PointIndex(0,1,7,8),};
            PointIndex[] sorted = PointIndex.SortByRing(points);

            Assert.IsTrue(sorted.Take(6).All(p => p.IsInner == false));
            Assert.IsTrue(sorted.Skip(6).All(p => p.IsInner));
            Assert.IsTrue(sorted.First().iVertex == 4);
            Assert.IsTrue(sorted[1].iVertex == 5);
            Assert.IsTrue(sorted[2].iVertex == 7);
            Assert.IsTrue(sorted[5].iVertex == 2);

            Assert.IsTrue(sorted[6].iVertex == 4);
            Assert.IsTrue(sorted[7].iVertex == 5);
            Assert.IsTrue(sorted[8].iVertex == 7);
            Assert.IsTrue(sorted[11].iVertex == 2);

        }

        [TestMethod]
        public void Theorem4Test()
        {
            GridLineSegment line;
            GridPolygon U = CreateUPolygon(10);

            //Line passes along the entire length of exterior ring
            line = new GridLineSegment(new GridVector2(-11, -10), new GridVector2(11, -10));
            Assert.IsTrue(Theorem4(U, line));

            //Line passes through part of the lenght of exterior ring
            line = new GridLineSegment(new GridVector2(-9, -10), new GridVector2(11, -10));
            Assert.IsTrue(Theorem4(U, line));

            //Line crosses the exterior ring
            line = new GridLineSegment(new GridVector2(-9, -11), new GridVector2(-9, -9));
            Assert.IsFalse(Theorem4(U, line));
        }


        /// <summary>
        /// Theorem 4 requries that a line segment does not occupy space both internal and external to the polygon.
        /// Lines that fall over a polygon segment are acceptable as long as the rest of the line qualifies.
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Theorem4(GridPolygon poly, GridLineSegment line)
        {
            List<GridVector2> intersections;

            return !LineIntersectionExtensions.Intersects(line, poly, true, out intersections);
        }
        
    }
}
