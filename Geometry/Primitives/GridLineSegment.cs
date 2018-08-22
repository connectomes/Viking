﻿using System;
using System.Diagnostics; 
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geometry
{ 
        [Serializable]
    public struct GridLineSegment : IComparable, ICloneable, IComparer<GridLineSegment>, ILineSegment2D, IEquatable<GridLineSegment>
    {
        public readonly GridVector2 A;
        public readonly GridVector2 B;

        public GridLineSegment(IPoint2D A, IPoint2D B)
        {
            /* This is a bad idea because callers expect A and B to maintain position
            int diff = A.Compare(A, B);
            this.A = diff <= 0 ? A : B;
            this.B = diff <= 0 ? B : A;
            */
            this.A = A.Convert();
            this.B = B.Convert();

            if (A == B)
            {
                throw new ArgumentException("Can't create line with two identical points");
            }
        }

        public GridLineSegment(GridVector2 A, GridVector2 B)
        {
            /* This is a bad idea because callers expect A and B to maintain position
            int diff = A.Compare(A, B);
            this.A = diff <= 0 ? A : B;
            this.B = diff <= 0 ? B : A;
            */
            this.A = A;
            this.B = B; 

            if (A == B)
            {
                throw new ArgumentException("Can't create line with two identical points");
            }
        }

        /// <summary>
        /// Create an array of grid line segments connecting the array of points in order
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static GridLineSegment[] SegmentsFromPoints(GridVector2[] points)
        {
            if (points.Length < 2)
                throw new ArgumentException("Not enough points to create GridLineSegment array");
             
            GridLineSegment[] segs = new GridLineSegment[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                segs[i] = new GridLineSegment(points[i], points[i + 1]);
            }

            return segs;
        }

        public override string ToString()
        {
            if(MinX == A.X)
                return A.X.ToString("F") + " " + A.Y.ToString("F2") + " , " + B.X.ToString("F2") + " " + B.Y.ToString("F2");
            else
                return B.X.ToString("F") + " " + B.Y.ToString("F2") + " , " + A.X.ToString("F2") + " " + A.Y.ToString("F2");
            
        }

        object ICloneable.Clone()
        {
            return new GridLineSegment(A, B);
        }

        public int Compare(GridLineSegment SegA, GridLineSegment SegB)
        {
            double diff = SegA.MinX - SegB.MinX;

            if (diff == 0.0)
            {
                diff = SegA.MinY - SegB.MinY;
                if (diff == 0.0)
                {
                    diff = SegA.MaxX - SegB.MaxX;
                    if (diff == 0.0)
                    {
                        diff = SegA.MaxY - SegB.MaxY;
                    }
                }
            }

            if (diff > 0)
                return 1;
            if (diff < 0)
                return -1;

            return 0;
        }

        public int CompareTo(object obj)
        {
            GridLineSegment SegB = (GridLineSegment)obj;

            return Compare(this,SegB); 
        }

        public override int GetHashCode()
        {
            return (int)MinX; 
            /*
            int CodeA = A.GetHashCode();
            int CodeB = B.GetHashCode();
            int Code;

            try
            {
                Code = CodeA + CodeB;
            }
            catch (OverflowException e)
            {
                Code = CodeA; 
            }

            return Code; */
        }

        public override bool Equals(object obj)
        { 
            GridLineSegment ls;
//            try
//            {
                ls = (GridLineSegment)obj;
            //            }
            //            catch(System.InvalidCastException e)
            //            {
            //                return false; 
            //            }
            /*
            if (A.X == ls.A.X &&
                A.Y == ls.A.Y &&
                B.X == ls.B.X &&
                B.Y == ls.B.Y)
                return true;
            */
            if (object.ReferenceEquals(obj, null))
                return false;

            if (this.A == ls.A && this.B == ls.B)
                return true;

            if (this.A == ls.B && this.B == ls.A)
                return true;

            /*
                if (MaxX == ls.MaxX &&
                   MaxY == ls.MaxY &&
                   MinX == ls.MinX &&
                   MinY == ls.MinY)
                    return true; 
                    */

            return false; 
        }

        static public bool operator ==(GridLineSegment A, GridLineSegment B)
        {
            if (object.ReferenceEquals(A, B))
                return true;

            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return false;

            if (A.A == B.A && A.B == B.B)
                return true;

            if (A.A == B.B && A.B == B.A)
                return true;
            
            return false; 
        }

        static public bool operator !=(GridLineSegment A, GridLineSegment B)
        {
            return !(A == B);
        }


        public double Length
        {
            get
            {
                double d1 = (A.X - B.X);
                d1 = d1 * d1;
                double d2 = (A.Y - B.Y);
                d2 = d2 * d2; 

                return Math.Sqrt(d1+d2); 
            }
        }

        /// <summary>
        /// The change in Y for values of X.
        /// Returns NAN if the line is vertical
        /// </summary>
        public double slope
        {
            get
            {
                if (A.X == B.X)
                    return double.NaN;
                else
                {
                    double YDelta = B.Y - A.Y; 
                    double XDelta = B.X - A.X; 
                    return YDelta / XDelta; 
                }
            }
        }

        /// <summary>
        /// The point where the line intercepts the y-axis, returns NAN if the line is vertical
        /// </summary>
        public double intercept
        {
            get
            {
                if (A.X == B.X)
                    return double.NaN;

                return ((A.Y * B.X) - (B.Y * A.X)) / (B.X - A.X); 

            }
        }

        /// <summary>
        /// The change in Y for values of X.
        /// Returns NAN if the line is vertical
        /// </summary>
        public double yslope
        {
            get
            {
                return 1 / slope; 
            }
        }

        /// <summary>
        /// The point where the line intercepts the y-axis, returns NAN if the line is vertical
        /// </summary>
        public double yintercept
        {
            get
            {
                if (A.Y == B.Y)
                    return double.NaN;

                return A.X - (yslope * A.Y);

            }
        }

        /// <summary>
        /// Return true if either point at each end of the line matches an endpoint of the passed segment
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="Endpoint"></param>
        /// <returns></returns>
        public bool SharedEndPoint(GridLineSegment seg)
        {
            bool AMatch = A == seg.A || A == seg.B;
            bool BMatch = B == seg.A || B == seg.B;

            return AMatch || BMatch;
        }

        /// <summary>
        /// Return true if either point at each end of the line matches an endpoint of the passed segment
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="Endpoint"></param>
        /// <returns></returns>
        public bool SharedEndPoint(GridLineSegment seg, out GridVector2 Endpoint)
        {
            bool AMatch = A == seg.A || A == seg.B;
            bool BMatch = B == seg.A || B == seg.B;
             
            if(AMatch || BMatch)
            {
                Endpoint = AMatch ? A : B;
                return true;
            }
            else
            {
                Endpoint = GridVector2.Zero;
                return false;
            }
        }

        public bool IsEndpoint(IPoint2D p)
        {
            return A == p || B == p;
        }

        /// <summary>
        /// Return true if point p is to left when standing at A looking towards B
        /// </summary>
        /// <param name="p"></param>
        /// <returns> 1 for left
        ///           0 for on the line
        ///           -1 for right
        /// </returns>
        public int IsLeft(GridVector2 p)
        {
            return Math.Sign((B.X - A.X) * (p.Y - A.Y) - (B.Y - A.Y) * (p.X - A.X));
        }

        public GridVector2 OppositeEndpoint(GridVector2 p)
        {
            return A == p ? B : A;
        }

        public GridVector2 Bisect()
        {
            double x = (A.X + B.X) / 2.0;
            double y = (A.Y + B.Y) / 2.0;

            return new GridVector2(x, y); 
        }

        public GridVector2 Direction
        {
            get
            {
                GridVector2 D = B - A;
                return GridVector2.Normalize(D);
            }
        }


        public bool Contains(GridVector2 p)
        {
            return Math.Abs(this.DistanceToPoint(p)) < Global.Epsilon;
        }

        /// <summary>
        /// Project the point p onto the line
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double Dot(GridVector2 p)
        {
            return GridVector2.Dot(p - A, B - A);
        }

        /// <summary>
        /// Return a normal to the line, the returned vector is normalized
        /// </summary>
        public GridVector2 Normal
        {
            get
            {
                GridVector2 delta = B - A;

                GridVector2 normal = new GridVector2(-delta.Y, delta.X);
                normal.Normalize();
                return normal;
            }
        }

        public double DistanceToPoint(GridVector2 point)
        {
            GridVector2 temp;
            return DistanceToPoint(point, out temp);
        }

        /// <summary>
        /// The point on the segment at a fractional distance between A & B
        /// </summary>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public GridVector2 PointAlongLine(double fraction)
        {
            GridVector2 delta = B - A;
            delta *= fraction;

            return A + delta;
        }
          
        internal static bool NearlyZero(double value)
        {
            return (value < Global.Epsilon && value > -Global.Epsilon);
        }

        /// <summary>
        /// To find the nearest point to a line we project the point onto the infinite line along the line segment.  This function indicates if the point falls beyond the boundaries of the line segment.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>True if proejected point lands within line segment</returns>
        public bool IsNearestPointWithinLineSegment(GridVector2 point)
        {
            double DX = B.X - A.X;
            double DY = B.Y - A.Y;

            /*Special case for horizontal or vertical lines*/
            if (NearlyZero(DX))
            {
                //Point is between line segment
                return point.Y <= MaxY && point.Y >= MinY;
            }
            else if (NearlyZero(DY))
            {
                //Point is between line segment
                return point.X <= MaxX && point.X >= MinX;
            }

            //Line is at an angle.  Find the intersection
            double t = ((point.X - A.X) * DX + (point.Y - A.Y) * DY) / (DX * DX + DY * DY);

            //Make sure t value is on the line 
            return t >= 0 && t <= 1.0;                
        }

        /// <summary>
        /// Returns the distance of the line to the specified point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="Intersection"></param>
        /// <returns></returns>
        public double DistanceToPoint(GridVector2 point, out GridVector2 Intersection)
        {
            double DX = B.X - A.X;
            double DY = B.Y - A.Y;

            /*Special case for horizontal or vertical lines*/
            if (NearlyZero(DX))
            {
                //Point is between line segment
                if (point.Y <= MaxY &&
                   point.Y >= MinY)
                {
                    Intersection = new GridVector2(A.X, point.Y);
                    return Math.Abs(point.X - A.X);
                }
                if (point.Y > MaxY) //Point is above line segment, calculate distance
                {
                    Intersection = new GridVector2(A.X, MaxY);
                    return GridVector2.Distance(point, Intersection);
                }
                else //(Point.Y < MinY) //Point is below line segment, calculate distance
                {
                    Intersection = new GridVector2(A.X, MinY);
                    return GridVector2.Distance(point, Intersection);
                }
            }
            else if (NearlyZero(DY))
            {
                //Point is between line segment
                if (point.X <= MaxX &&
                   point.X >= MinX)
                {
                    Intersection = new GridVector2(point.X, A.Y);
                    return Math.Abs(point.Y - A.Y);
                }
                if (point.X > MaxX) //Point is to right of line segment, calculate distance
                {
                    Intersection = new GridVector2(MaxX, A.Y);
                    return GridVector2.Distance(point, new GridVector2(MaxX, A.Y));
                }
                else //(Point.X < MinX) //Point is to left of line segment, calculate distance
                {
                    Intersection = new GridVector2(MinX, A.Y);
                    return GridVector2.Distance(point, new GridVector2(MinX, A.Y));
                }
            }

            //Line is at an angle.  Find the intersection
            double t = ((point.X - A.X) * DX + (point.Y - A.Y) * DY) / (DX * DX + DY * DY); 

            //Make sure t value is on the line
            double tOnTheLine = Math.Min(Math.Max(0, t), 1);

            if (tOnTheLine > 0 && tOnTheLine < 1.0)
            {
                Intersection = new GridVector2(A.X + t * DX, A.Y + t * DY);
                return GridVector2.Distance(point, Intersection);
            }
            else //Return the endpoint of the segment the point is closest to
            {
                double DistA = GridVector2.Distance(point, A);
                double DistB = GridVector2.Distance(point, B);
                Intersection = DistA < DistB ? A : B; 
                return DistA < DistB ? DistA : DistB; 
            }
        }

        public bool Intersects(GridLineSegment seg)
        {
            IShape2D intersection;
            return this.Intersects(seg, out intersection);
        }

        public bool Intersects(GridLineSegment seg, bool EndpointsOnRingDoNotIntersect)
        {
            IShape2D intersection;
            return this.Intersects(seg, EndpointsOnRingDoNotIntersect, out intersection);
        }

        public bool Intersects(GridLineSegment seg, bool EndpointsOnRingDoNotIntersect, out IShape2D Intersection)
        { 
            bool intersects = this.Intersects(seg, out Intersection); 

            if(intersects && EndpointsOnRingDoNotIntersect)
            {
                if (Intersection.ShapeType == ShapeType2D.POINT)
                { 
                    return !seg.IsEndpoint((IPoint2D)Intersection);
                }
                else if(Intersection.ShapeType == ShapeType2D.LINE)
                {
                    return true;
                }

                Debug.Fail("We should not be able to reach this case, a line intersection is either a point or a line");
                return true;
            }

            return intersects;
        }

        public bool Intersects(GridLineSegment seg, out GridVector2 Intersection)
        {
            IShape2D shape;  
            Intersection = new GridVector2();
            bool intersects = this.Intersects(seg, out shape);
            if(intersects)
            {
                if (shape.ShapeType == ShapeType2D.POINT)
                {
                    Intersection = (GridVector2)shape;
                    return true;
                }
                else if (shape.ShapeType == ShapeType2D.LINE)
                {
                    Intersection = (GridVector2)(((ILineSegment2D)shape).A);
                    return true;
                }

                Debug.Fail("We should not be able to reach this case, a line intersection is either a point or a line");
                return true;
            }

            return intersects;
        }

        public bool Intersects(GridLineSegment seg, out IShape2D Intersection)
        {
            //Don't do the full check if the bounding boxes don't overlap
            if (!this.BoundingBox.Intersects(seg.BoundingBox))
            {
                Intersection = new GridVector2();
                return false;
            }

            //Function for each line
            //Ax + By = C

            double A1 = B.Y - A.Y;
            double A2 = seg.B.Y - seg.A.Y;

            double B1 = A.X - B.X;
            double B2 = seg.A.X - seg.B.X;
            
            double C1 = A1*A.X + B1 * A.Y; 
            double C2 = A2*seg.A.X + B2 * seg.A.Y; 

            double det = A1 * B2 - A2 * B1;
            //Check if lines are parallel
            if (det == 0)
            {
                Intersection = GridVector2.Zero;

                //Check if the distance to the endpoint of the other line is zero
                GridRectangle? overlapRect = this.BoundingBox.Intersection(seg.BoundingBox);
                if (!overlapRect.HasValue)
                {
                   
                    return false;
                }

                //Check if the lines perfectly overlap
                GridVector2[] endpoints = new GridVector2[] { seg.A, seg.B, this.A, this.B }.Distinct().ToArray();
                GridVector2[] endpointsOnLine = endpoints.Where(e => overlapRect.Value.Contains(e) && seg.DistanceToPoint(e) < Global.Epsilon).ToArray();

                Debug.Assert(endpointsOnLine.Length > 0, "Must have intersecting points if the bounding boxes overlap for parallel line intersection test");
                if(endpointsOnLine.Length == 0)
                {
                    return false;
                }
                else if(endpointsOnLine.Length == 1)
                {
                    Intersection = endpointsOnLine[0];
                    return true;
                }
                else if(endpointsOnLine.Length == 2)
                {
                    Intersection = new GridLineSegment(endpointsOnLine[0], endpointsOnLine[1]);
                    return true;
                }
                else
                {
                    GridVector2[] endpointsOnOverlapRect = endpointsOnLine.Where(e => overlapRect.Value.Corners.Contains(e)).ToArray();
                    Intersection = new GridLineSegment(endpointsOnOverlapRect[0], endpointsOnOverlapRect[1]);
                    return true;
                }
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;

                Intersection = new GridVector2(x, y);

                double minX = Math.Min(A.X, B.X) - Global.Epsilon;
                double minSegX = Math.Min(seg.A.X, seg.B.X) - Global.Epsilon;

                if (minX > x || minSegX > x)
                    return false;

                double maxX = Math.Max(A.X, B.X) + Global.Epsilon; 
                double maxSegX = Math.Max(seg.A.X, seg.B.X) + Global.Epsilon;

                if (maxX < x || maxSegX < x)
                    return false;

                double minY = Math.Min(A.Y, B.Y)- Global.Epsilon;
                double minSegY = Math.Min(seg.A.Y, seg.B.Y) - Global.Epsilon;

                if (minY > y || minSegY > y)
                    return false;

                double maxY = Math.Max(A.Y, B.Y) + Global.Epsilon;
                double maxSegY = Math.Max(seg.A.Y, seg.B.Y) + Global.Epsilon;

                if (maxY < y || maxSegY < y)
                    return false;

                return true;
            }
        }

        public bool Intersects(IEnumerable<GridLineSegment> seg)
        {
            GridLineSegment line = this;
            return seg.Any(ls => line.Intersects(ls));
        }


        public bool Intersects(IShape2D shape)
        {
            return ShapeExtensions.LineIntersects(this, shape);
        }


        public bool Intersects(ICircle2D c)
        {
            GridCircle circle = c.Convert();
            return this.Intersects(circle);
        }

        public bool Intersects(GridCircle circle)
        {
            return LineIntersectionExtensions.Intersects(this, circle);
        }


        public bool Intersects(ILineSegment2D l)
        {
            GridLineSegment line = l.Convert();
            return this.Intersects(line);
        }
         
        public bool Intersects(ITriangle2D t)
        {
            GridTriangle tri = t.Convert();
            return this.Intersects(tri);
        }

        public bool Intersects(GridTriangle tri)
        {
            return LineIntersectionExtensions.Intersects(this, tri);
        }

        public bool Intersects(IPolygon2D p)
        {
            GridPolygon poly = p.Convert();
            return this.Intersects(poly);
        }

        public bool Intersects(GridPolygon poly)
        {
            return LineIntersectionExtensions.Intersects(this, poly);
        }

        public double MinX
        {
            get
            {
                return A.X < B.X ? A.X : B.X;
            }
        }

        public double MaxX
        {
            get
            {
                return A.X > B.X ? A.X : B.X;
            }
        }

        public double MinY
        {
            get
            {
                return A.Y < B.Y ? A.Y : B.Y;
            }
        }

        public double MaxY
        {
            get
            {
                return A.Y > B.Y ? A.Y : B.Y;
            }
        }

        public GridRectangle BoundingBox
        {
            get
            {
                return new GridRectangle(MinX, MaxX, MinY, MaxY);
            }
        }

        IPoint2D ILineSegment2D.A
        {
            get
            {
                return this.A;
            }
        }

        IPoint2D ILineSegment2D.B
        {
            get
            {
                return this.B;
            }
        }

        public double Area
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ShapeType2D ShapeType
        {
            get
            {
                return ShapeType2D.LINE;
            }
        }

        public GridLine ToLine()
        {
            return new GridLine(this.A, this.Direction);
        }

        public bool Contains(IPoint2D p)
        {
            return this.Contains(p.Convert());
        }

        public IShape2D Translate(IPoint2D offset)
        {
            return this.Translate(offset.Convert());
        }

        public GridLineSegment Translate(GridVector2 offset)
        {
            return new GridLineSegment(this.A + offset, this.B + offset);
        }

        bool IEquatable<GridLineSegment>.Equals(GridLineSegment other)
        {
            return this == other;
        }
    }
}
