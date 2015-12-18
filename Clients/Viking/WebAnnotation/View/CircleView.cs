﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Geometry;
using VikingXNAGraphics;

namespace WebAnnotation.View
{
      
    public class TextureCircleView : CircleView
    {
        public Texture2D Texture;
        bool FlipTexture = false;

        public TextureCircleView(Texture2D texture, GridCircle circle) : base()
        {
            this.Circle = circle;
            this.Texture = texture;
        }

        public static TextureCircleView CreateUpArrow(GridCircle circle)
        {
            return new TextureCircleView(GlobalPrimitives.UpArrowTexture, circle);
        }

        public static TextureCircleView CreateDownArrow(GridCircle circle)
        {
            TextureCircleView view = new TextureCircleView(GlobalPrimitives.UpArrowTexture, circle);
            view.FlipTexture = true; 
            return view;
        }

        public override VertexPositionColorTexture[] BackgroundVerts
        {
            get
            {
                if (_BackgroundVerts == null)
                {
                    _BackgroundVerts = CircleView.VerticiesForCircle(this.Circle);
                    if(FlipTexture)
                        this.FlipTextureY();
                }

                return _BackgroundVerts;
            }
        }


        public static void SetupGraphicsDevice(GraphicsDevice device, BasicEffect basicEffect, VikingXNA.AnnotationOverBackgroundLumaEffect overlayEffect)
        {
            //Note one still needs to set the texture for the effect before rendering after calling this method
            DeviceStateManager.SaveDeviceState(device);
            DeviceStateManager.SetRenderStateForShapes(device);
            DeviceStateManager.SetRasterizerStateForShapes(device);

            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
        }

        public static void RestoreGraphicsDevice(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            DeviceStateManager.RestoreDeviceState(graphicsDevice);

            basicEffect.Texture = null;
            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = false;
        }

        private void FlipTextureY()
        {
            if (BackgroundVerts == null)
                return; 

            for (int i = 0; i < BackgroundVerts.Length; i++)
            {
                _BackgroundVerts[i].TextureCoordinate = new Vector2(_BackgroundVerts[i].TextureCoordinate.X == 0 ? 1 : 0,
                                                                                        _BackgroundVerts[i].TextureCoordinate.Y == 0 ? 1 : 0);
            }
        }

        public static void Draw(GraphicsDevice device,
                          VikingXNA.Scene scene,
                          BasicEffect basicEffect,
                          VikingXNA.AnnotationOverBackgroundLumaEffect overlayEffect,
                          TextureCircleView[] listToDraw)
        {
            if (listToDraw.Length == 0)
                return;

            TextureCircleView.SetupGraphicsDevice(device, basicEffect, overlayEffect);

            var textureGroups = listToDraw.GroupBy(l => l.Texture);
            foreach(var textureGroup in textureGroups)
            {
                TextureCircleView[] views = textureGroup.ToArray();
                overlayEffect.AnnotateWithTexture(textureGroup.Key);
                basicEffect.Texture = textureGroup.Key;

                int[] indicies;
                VertexPositionColorTexture[] VertArray = AggregatePrimitives(views, out indicies);

                foreach (EffectPass pass in overlayEffect.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                                                                                         VertArray,
                                                                                         0,
                                                                                         VertArray.Length,
                                                                                         indicies,
                                                                                         0,
                                                                                         indicies.Length / 3);
                }
            }

            TextureCircleView.RestoreGraphicsDevice(device, basicEffect);
        }
    }
     

    public class CircleView
    {
        #region static

        static double BeginFadeCutoff = 0.1;
        static double InvisibleCutoff = 1f;

        #endregion

        private GridCircle _Circle;
        public GridCircle Circle
        {
            get
            {
                return _Circle;
            }
            set
            {
                ClearCachedData();
                _Circle = value;
            }
        }

        public GridVector2 VolumePosition
        {
            get
            {
                return _Circle.Center;
            }

        }

        public double Radius
        {
            get { return _Circle.Radius; }
        }

        public double OffSectionRadius
        {
            get { return this.Radius / 2.0; }
        }

        private Microsoft.Xna.Framework.Color _BackgroundColor;
        public Microsoft.Xna.Framework.Color BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                _BackgroundColor = value;
                _BackgroundHSLColor = value.ConvertToHSL();
                ClearCachedData();
            }
        }

        private Microsoft.Xna.Framework.Color _BackgroundHSLColor;
        public Microsoft.Xna.Framework.Color BackgroundHSLColor
        {
            get
            {
                return _BackgroundHSLColor;
            }
        }

        /// <summary>
        /// Called when we have changed a property that affects rendering
        /// </summary>
        public void ClearCachedData()
        {
            _BackgroundVerts = null;
        }

        /// <summary>
        /// Return true if the circle would be visible if rendered into the scene
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public bool IsVisible(VikingXNA.Scene scene)
        {
            double maxDimension = Math.Max(scene.VisibleWorldBounds.Width, scene.VisibleWorldBounds.Height);
            double LocToScreenRatio = Radius * 2 / maxDimension;
            if (LocToScreenRatio > InvisibleCutoff)
                return false;

            double maxPixelDimension = Math.Max(scene.DevicePixelWidth, scene.DevicePixelHeight);
            if (Radius * 2 <= maxPixelDimension)
                return false;

            return true;
        }

        #region Render Code

        /// <summary>
        /// Create billboard primitive the size and position of the circle
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="Verts"></param>
        /// <returns></returns>
        protected static VertexPositionColorTexture[] VerticiesForCircle(GridCircle circle)
        {
            VertexPositionColorTexture[] Verts = new VertexPositionColorTexture[GlobalPrimitives.SquareVerts.Length];
            GlobalPrimitives.SquareVerts.CopyTo(Verts, 0);

            for (int i = 0; i < Verts.Length; i++)
            {
                Verts[i].Position *= (float)circle.Radius; 
                Verts[i].Position.X += (float)circle.Center.X;
                Verts[i].Position.Y += (float)circle.Center.Y;
            }

            return Verts;
        }

        protected VertexPositionColorTexture[] _BackgroundVerts = null;
        public virtual VertexPositionColorTexture[] BackgroundVerts
        {
            get
            {
                if (_BackgroundVerts == null)
                {
                    _BackgroundVerts = VerticiesForCircle(this.Circle);
                }

                return _BackgroundVerts;
            }
        }

        public static float BaseSaturationScalar(bool MouseOver, bool OnVisibleSection)
        {
            if (MouseOver)
            {
                return 0.25f;
            }
            else if (!OnVisibleSection)
            {
                return 0.5f;
            }

            return 1.0f;
        }

        public static float BaseAlpha(bool MouseOver, bool OnVisibleSection)
        {
            if (MouseOver)
            {
                return 0.125f;
            }
            else if (!OnVisibleSection)
            {
                return 0.25f;
            }

            return 0.5f;
        }

        public static Microsoft.Xna.Framework.Color AdjustHSLColorForStatus(Microsoft.Xna.Framework.Color HSLColor, GridRectangle VisibleBounds, double Radius, bool MouseOver, bool OnVisibleSection)
        {

            float SatScalar = BaseSaturationScalar(MouseOver, OnVisibleSection);//HSLColor.B / 255.0f;
            double maxDimension = Math.Max(VisibleBounds.Width, VisibleBounds.Height);
            double LocToScreenRatio = Radius * 2 / maxDimension;
            SatScalar *= Viking.Common.Util.GetFadeFactor(LocToScreenRatio, BeginFadeCutoff, InvisibleCutoff);

            HSLColor.A = (Byte)((float)BaseAlpha(MouseOver, OnVisibleSection));
            HSLColor.G = (Byte)((float)HSLColor.G * SatScalar);
            return HSLColor;
        }

        /// <summary>
        /// The verticies should really be cached and handed up to LocationObjRenderer so all similiar objects can be rendered in one
        /// call.  This method is in the middle of a change from using triangles to draw circles to using textures. 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="DirectionToVisiblePlane"></param>
        /// <param name="color"></param>
        public VertexPositionColorTexture[] GetCircleBackgroundVerts(Microsoft.Xna.Framework.Color HSLColor, out int[] indicies)
        {
            //            GridVector2 Pos = this.VolumePosition;

            //Can't populate until we've referenced CircleVerts
            indicies = GlobalPrimitives.SquareIndicies;
            //            float radius = (float)this.Radius;

            VertexPositionColorTexture[] verts = BackgroundVerts;

            //Draw an opaque border around the background
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i].Color = HSLColor;
            }

            return verts;
        }

        public static void SetupGraphicsDevice(GraphicsDevice device, BasicEffect basicEffect, VikingXNA.AnnotationOverBackgroundLumaEffect overlayEffect)
        {
            DeviceStateManager.SaveDeviceState(device);
            DeviceStateManager.SetRenderStateForShapes(device);
            DeviceStateManager.SetRasterizerStateForShapes(device);

            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            if(overlayEffect != null)
                overlayEffect.AnnotateWithCircle((float)0.05);
        }

        public static void RestoreGraphicsDevice(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            DeviceStateManager.RestoreDeviceState(graphicsDevice);

            basicEffect.Texture = null;
            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = false;
        }

        protected static VertexPositionColorTexture[] AggregatePrimitives(CircleView[] listToDraw, out int[] indicies)
        {
            VertexPositionColorTexture[] VertArray = new VertexPositionColorTexture[listToDraw.Length * 4];
            indicies = new int[listToDraw.Length * 6];

            int iNextVert = 0;
            int iNextVertIndex = 0;

            for (int iObj = 0; iObj < listToDraw.Length; iObj++)
            {
                CircleView locToDraw = listToDraw[iObj];
                int[] locIndicies;
                VertexPositionColorTexture[] objVerts = locToDraw.GetCircleBackgroundVerts(locToDraw.BackgroundHSLColor, out locIndicies);

                if (objVerts == null)
                    continue;

                Array.Copy(objVerts, 0, VertArray, iNextVert, objVerts.Length);

                for (int iVert = 0; iVert < locIndicies.Length; iVert++)
                {
                    indicies[iNextVertIndex + iVert] = locIndicies[iVert] + iNextVert;
                }

                iNextVert += objVerts.Length;
                iNextVertIndex += locIndicies.Length;
            }

            return VertArray;
        }


        public static void Draw(GraphicsDevice device,
                          VikingXNA.Scene scene,
                          BasicEffect basicEffect,
                          VikingXNA.AnnotationOverBackgroundLumaEffect overlayEffect,
                          CircleView[] listToDraw)
        {
            if (listToDraw.Length == 0)
                return;

            CircleView.SetupGraphicsDevice(device, basicEffect, overlayEffect);
            int[] indicies;
            VertexPositionColorTexture[] VertArray = AggregatePrimitives(listToDraw, out indicies);

            foreach (EffectPass pass in overlayEffect.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                                                                                     VertArray,
                                                                                     0,
                                                                                     VertArray.Length,
                                                                                     indicies,
                                                                                     0,
                                                                                     indicies.Length / 3);
            }

            CircleView.RestoreGraphicsDevice(device, basicEffect);
        }
        #endregion
    }
}