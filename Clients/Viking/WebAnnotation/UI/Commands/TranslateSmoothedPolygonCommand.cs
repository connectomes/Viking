﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnnotationModel;
using Geometry;
using WebAnnotation.View;
using Viking.VolumeModel;
using SqlGeometryUtils;
using VikingXNAGraphics;
using System.Windows.Forms;
using System.Diagnostics;
using WebAnnotation;
using System.Collections.ObjectModel;
using VikingXNAWinForms;
using Microsoft.Xna.Framework.Graphics;

namespace WebAnnotation.UI.Commands
{
    class TranslatePolygonCommand : RotateTranslateScaleCommand, Viking.Common.IHelpStrings
    {
        public delegate void OnCommandSuccess(GridPolygon MosaicPolygon);
        protected OnCommandSuccess success_callback;
          
        public override string[] HelpStrings
        {
            get
            {
                List<string> s = new List<string>(base.HelpStrings);
                s.AddRange(TranslateOpenCurveCommand.DefaultMouseHelpStrings);
                s.Sort();
                return s.ToArray();
            }
        }

        protected GridVector2 DeltaSum = new GridVector2(0, 0); 

        private GridPolygon OriginalMosaicPolygon; 
        public GridPolygon TransformedMosaicPolygon;
        protected MeshModel<VertexPositionColor> _mesh;
        protected CircleView OriginalVolumePositionView;
        protected CircleView TranslatedVolumePositionView;

        public Microsoft.Xna.Framework.Color Color;

        /// <summary>
        /// True if the Polygon's boundaries should be smoothed with a curve fitting algorithm
        /// </summary>
        public bool SmoothPolygon = false; 

        protected override GridVector2 VolumeRotationOrigin
        {
            get
            {
                return mapping.SectionToVolume(TransformedMosaicPolygon.Centroid);
            }
        }

        public TranslatePolygonCommand(Viking.UI.Controls.SectionViewerControl parent,
                                        GridPolygon MosaicPolygon,
                                        GridVector2 VolumePosition,
                                        Microsoft.Xna.Framework.Color color,
                                        OnCommandSuccess success_callback) : base(parent, VolumePosition)
        {
            OriginalMosaicPolygon = MosaicPolygon;
            Color = color;
            mapping = parent.Section.ActiveSectionToVolumeTransform;
            TransformedMosaicPolygon = CalculateTransformedPolygon();
            CreateUpdateView();
            this.success_callback = success_callback;
        }

        public override void OnDraw(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice,
                                    VikingXNA.Scene scene,
                                    BasicEffect basicEffect)
        {
            CircleView.Draw(graphicsDevice, scene, basicEffect,
                            DeviceEffectsStore<AnnotationOverBackgroundLumaEffect>.GetOrCreateForDevice(graphicsDevice, Parent.Content),
                            new CircleView[] { OriginalVolumePositionView, TranslatedVolumePositionView });

            MeshView<VertexPositionColor>.Draw(graphicsDevice, scene, new MeshModel<VertexPositionColor>[] { _mesh });
        }

        protected override void OnAngleChanged()
        {
            TransformedMosaicPolygon = CalculateTransformedPolygon();
            CreateUpdateView();
        }

        protected override void OnSizeScaleChanged()
        {
            TransformedMosaicPolygon = CalculateTransformedPolygon();
            CreateUpdateView();
        }

        protected override void OnTranslationChanged()
        { 
            TransformedMosaicPolygon = CalculateTransformedPolygon();
            CreateUpdateView();
        }

        protected GridPolygon CalculateTransformedPolygon()
        {
            GridPolygon poly;
            poly = OriginalMosaicPolygon.Rotate(this.Angle);
            poly = poly.Scale(this.SizeScale);
            poly = poly.Translate(this.MosaicPositionDeltaSum);
            return poly;
        }

        protected void CreateUpdateView()
        {
            GridPolygon TransformedVolumePolygon = mapping.TryMapShapeSectionToVolume(this.TransformedMosaicPolygon);
            TransformedVolumePolygon = TransformedVolumePolygon.Smooth(Global.NumClosedCurveInterpolationPoints);
            _mesh = TransformedVolumePolygon.CreateMeshForPolygon2D(Color.ConvertToHSL());

            OriginalVolumePositionView = new CircleView(new GridCircle(this.OriginalVolumePosition, 16), Microsoft.Xna.Framework.Color.Red);
            TranslatedVolumePositionView = new CircleView(new GridCircle(this.TranslatedVolumePosition, 16), Microsoft.Xna.Framework.Color.Green);
        }

        protected override void Execute()
        {
            if( this.success_callback != null)
            {
                /*
                GridPolygon VolumeShape = null;
                try
                {
                    VolumeShape = mapping.TryMapShapeSectionToVolume(this.TransformedMosaicPolygon);
                }
                catch(ArgumentOutOfRangeException)
                {
                    Trace.WriteLine("TranslateSmoothedPolygonCommand: Could not map polygon on Execute: " + TranslatedVolumePosition.ToString(), "Command");
                    return;
                }
                */
                success_callback(TransformedMosaicPolygon);
            }

            base.Execute();
        }
    }
}
