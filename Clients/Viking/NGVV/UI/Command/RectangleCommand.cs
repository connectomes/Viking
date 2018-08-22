﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VikingXNA;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Geometry;
using VikingXNAGraphics;
using System.Collections.ObjectModel;
using VikingXNAWinForms;

namespace Viking.UI.Commands
{
    public class RectangleCommand : DefaultCommand, Viking.Common.IHelpStrings, Viking.Common.IObservableHelpStrings
    {
        protected GridVector2 Origin;
        protected Geometry.GridRectangle MyRect;
        protected Microsoft.Xna.Framework.Color Color; 

        public override string[] HelpStrings
        {
            get
            {
                return new string[] {
                    "Left click to set origin of rectangle",
                    "Drag mouse to size rectangle",
                    "Release left button to finish rectangle"};
            }
        }

        public override ObservableCollection<string> ObservableHelpStrings
        {
            get
            {
                return new ObservableCollection<string>(this.HelpStrings);
            }
        }

        public RectangleCommand(Viking.UI.Controls.SectionViewerControl parent)
            : base(parent)
        {

        }

        protected override void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GridVector2 NewPosition = Parent.ScreenToWorld(e.X, e.Y);

            //Figure out if we are starting a rectangle
            if (e.Button.Left() && false == this.CommandActive)
            {
                Origin = NewPosition;
 //               MyRect = new Quad(Origin, 0, 0); 
               
                this.CommandActive = true; 
            }

            base.OnMouseDown(sender, e);
        }

        private bool TryUpdateMyRect(GridVector2 NewPosition)
        {
            try
            {
                MyRect = new GridRectangle(NewPosition, Origin); 
                return true;
            }
            catch (ArgumentException)
            {
               
            }
            return false;
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            //Figure out if we are starting a rectangle
            if (e.Button.Left() && this.CommandActive)
            {
                GridVector2 NewPosition = Parent.ScreenToWorld(e.X, e.Y);

                if(TryUpdateMyRect(NewPosition))
                {
                    this.Color = Color.Red;
                    this.Parent.Refresh();
                } 
            }

            base.OnMouseMove(sender, e);
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (oldMouse != null)
            {
                if (oldMouse.Button.Left() && this.CommandActive)
                {
                    GridVector2 NewPosition = Parent.ScreenToWorld(e.X, e.Y);
                    if(TryUpdateMyRect(NewPosition))
                    {
                        Execute();
                        this.Parent.Refresh();
                    }

                }
            }
            base.OnMouseUp(sender, e);
        }

        public override void OnDraw(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, VikingXNA.Scene scene, Microsoft.Xna.Framework.Graphics.BasicEffect basicEffect)
        {
            if (CommandActive == false)
                return;

            basicEffect.VertexColorEnabled = true; 
            basicEffect.TextureEnabled = false;            
            VertexPositionColor[] verts = new VertexPositionColor[] { new VertexPositionColor( new Vector3((float)MyRect.Left, (float)MyRect.Bottom, 1), Color.Yellow), 
                                                                       new VertexPositionColor( new Vector3((float)MyRect.Right, (float)MyRect.Bottom, 1), Color.Yellow), 
                                                                        new VertexPositionColor( new Vector3((float)MyRect.Right, (float)MyRect.Top, 1), Color.Yellow), 
                                                                         new VertexPositionColor( new Vector3((float)MyRect.Left, (float)MyRect.Top, 1), Color.Yellow)};

            Color CrossColor = new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, 0.25f);
            float EightWidth = (float)(MyRect.Width / 16);
            float EightHeight = (float)(MyRect.Height / 16);

            VertexPositionColor[] crossVerts = new VertexPositionColor[] { new VertexPositionColor( new Vector3((float)MyRect.Center.X - EightWidth, (float)MyRect.Center.Y, 1), CrossColor), 
                                                                       new VertexPositionColor( new Vector3((float)MyRect.Center.X + EightWidth, (float)MyRect.Center.Y, 1), CrossColor), 
                                                                        new VertexPositionColor( new Vector3((float)MyRect.Center.X, (float)MyRect.Center.Y - EightHeight, 1), CrossColor), 
                                                                         new VertexPositionColor( new Vector3((float)MyRect.Center.X, (float)MyRect.Center.Y + EightHeight, 1), CrossColor)};

            int[] indicies = new int[] { 0, 1, 2, 3, 0 };
            int[] crossIndicies = new int[] { 0, 1, 2, 3 }; 
            
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply(); 

                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, verts, 0, verts.Length, indicies, 0, indicies.Length - 1);
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, crossVerts, 0, crossVerts.Length, crossIndicies, 0, crossIndicies.Length / 2); 
            }

            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = false; 

            base.OnDraw(graphicsDevice,scene, basicEffect);
        }
    }
}
