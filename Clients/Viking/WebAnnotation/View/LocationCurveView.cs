﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using WebAnnotationModel;
using SqlGeometryUtils;
using Microsoft.Xna.Framework;

namespace WebAnnotation.View
{
    abstract class LocationCurveView : LocationLineViewBase, VikingXNAGraphics.IColorView
    {
        public abstract GridVector2[] MosaicCurveControlPoints { get; }
        public abstract GridVector2[] VolumeCurveControlPoints { get; }
        public abstract Color Color { get; set; }
        public abstract float Alpha { get; set; }

        public LocationCurveView(LocationObj obj, Viking.VolumeModel.IVolumeToSectionTransform mapper) : base(obj, mapper)
        { 
        }

        public override double DistanceFromCenterNormalized(GridVector2 Position)
        {
            if (PointIntersectsAnyControlPoint(Position))
            {
                return VolumeControlPoints.Select(p => GridVector2.Distance(p, Position) / ControlPointRadius).Min();
            }
            else
            {
                //TODO: Find a more accurate measurement.  Returning 0 means the line is always on top in selection.
                GridLineSegment[] segs = GridLineSegment.SegmentsFromPoints(this.VolumeCurveControlPoints);
                double MinDistance = segs.Min(l => l.DistanceToPoint(Position));
                return MinDistance / (this.LineWidth / 2.0);
            }
        }

        protected override bool PointIntersectsAnyLineSegment(GridVector2 WorldPosition)
        {
            //TODO: This could be optimized considerably
            GridLineSegment[] lineSegs = GridLineSegment.SegmentsFromPoints(this.VolumeCurveControlPoints);
            //Find the line segment the NewControlPoint intersects
            double MinDistance;
            int iNearest = lineSegs.NearestSegment(WorldPosition, out MinDistance);
            return MinDistance < this.LineWidth / 2.0f;
        }
    }
}
