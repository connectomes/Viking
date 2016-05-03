﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Viking.VolumeModel
{
    public static class RectangleMappingExtensions
    { 
        public static GridRectangle? ApproximateVisibleMosaicBounds(this GridRectangle VisibleWorldBounds, IVolumeToSectionTransform mapper)
        {
            GridVector2[] VolumeRectCorners = new GridVector2[] { VisibleWorldBounds.LowerLeft, VisibleWorldBounds.LowerRight, VisibleWorldBounds.UpperLeft, VisibleWorldBounds.UpperRight };
            GridVector2[] MosaicRectCorners;
            bool[] mapped = mapper.TryVolumeToSection(VolumeRectCorners, out MosaicRectCorners);

            GridVector2[] MappedMosaicCorners = MosaicRectCorners.Where((p, i) => mapped[i]).ToArray();

            if (MappedMosaicCorners.Length == 4)
            {
                //If we map at least three corners we know we can construct a reasonable approximation of the correct rectangle in mosaic space
                double MinX = MappedMosaicCorners.Min(p => p.X);
                double MaxX = MappedMosaicCorners.Max(p => p.X);
                double MaxY = MappedMosaicCorners.Max(p => p.Y);
                double MinY = MappedMosaicCorners.Min(p => p.Y);

                return new GridRectangle(MinX, MaxX, MinY, MaxY);
            }
            else if (MappedMosaicCorners.Length > 0)
            {
                //We mapped one or two points but not opposite corners.  Guesstimate the region by using the width/height in volume space since we know the mappings have minimal distortion.
                return EstimateMosaicRectangle(mapped, MosaicRectCorners, VisibleWorldBounds);
            } 

            return new GridRectangle?();
        }

        /// <summary>
        /// Assuming an array of length 4, with order (LowerLeft, LowerRight, UpperLeft, UpperLeft) with true values where points are mapped.
        /// Find or approximate the lower-left point
        /// </summary>
        /// <param name="mappedCorners"></param>
        /// <returns></returns>
        private static GridRectangle? EstimateMosaicRectangle(bool[] IsMapped, GridVector2[] points, GridRectangle VisibleWorldBounds)
        {  
            //If we map at least three corners we know we can construct a reasonable approximation of the correct rectangle in mosaic space
            GridVector2[] ValidPoints = points.Where((p, i) => IsMapped[i]).ToArray();
            double MinX = ValidPoints.Min(p => p.X);
            double MaxX = ValidPoints.Max(p => p.X);
            double MaxY = ValidPoints.Max(p => p.Y);
            double MinY = ValidPoints.Min(p => p.Y);

            //We don't know the rotation of the rectangle.  We assume the worst case of a 45 degree angle so we multiply width or height by 1.44
            //So we create a grid circle at the point with the radius of Max(Width,Height).  Then we return the bounding box of the GridCircle

            if(OppositeCornersMapped(IsMapped))
            {
                //Find the center of the opposite corners and the distance.  Create a circle and return the bounding box.
                if (IsMapped[0] && IsMapped[2])
                {
                    return CircleFromTwoPoints(points[0], points[2]).BoundingBox;
                }
                else
                {
                    return CircleFromTwoPoints(points[1], points[3]).BoundingBox;
                }
            }

            //Reaching here means only one point is mapped or two points on the same edge
            double CircleRadius = Math.Max(VisibleWorldBounds.Width, VisibleWorldBounds.Height) * 1.44; //Sqrt(2)
            for (int iPoint = 0; iPoint < points.Length; iPoint++)
            {
                if (IsMapped[0])
                {
                    return new GridCircle(points[0], CircleRadius).BoundingBox;
                }
            }

            return new GridRectangle?();
        }

        /// <summary>
        /// Return a circle bisected by points A, B with line AB passing through the center of the circle
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static GridCircle CircleFromTwoPoints(GridVector2 A, GridVector2 B)
        {
            double Distance = GridVector2.Distance(A,B);
            double X = (A.X + A.X) / 2.0;
            double Y = (B.Y + B.Y) / 2.0;

            return new GridCircle(new GridVector2(X, Y), Distance / 2.0);
        }

        /// <summary>
        /// Assuming an array of length 4, with order (LowerLeft, LowerRight, UpperLeft, UpperLeft) returns true if opposite corners are true
        /// </summary>
        /// <param name="mappedCorners"></param>
        /// <returns></returns>
        private static bool OppositeCornersMapped(bool[] mappedCorners)
        {
            return (mappedCorners[0] && mappedCorners[2]) || (mappedCorners[1] && mappedCorners[3]);
        }
    }
}