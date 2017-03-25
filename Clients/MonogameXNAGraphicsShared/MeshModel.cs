﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VikingXNAGraphics
{ 
    public interface IMeshModel<VERTEXTYPE>
         where VERTEXTYPE : struct, IVertexType
    {
        VERTEXTYPE[] Verticies { get; }
        int[] Edges { get; }
    }

    public class MeshModel<VERTEXTYPE> : IMeshModel<VERTEXTYPE>
        where VERTEXTYPE : struct, IVertexType
    { 
        public VERTEXTYPE[] Verticies
        {
            get;set;
        } 

        public int[] Edges
        {
            get;set;
        } 

        public MeshModel()
        {
        }
    }

    /// <summary>
    /// A helper class that assumes the entire mesh model is the same color
    /// </summary>
    public class PositionColorMeshModel : MeshModel<VertexPositionColor>, IColorView
    { 

        public PositionColorMeshModel()
        {
        }

        public float Alpha
        {
            get
            {
                return Verticies.First().Color.GetAlpha();
            }

            set
            {
                if (value != Alpha)
                {
                    Color newColor = this.Color.SetAlpha(value);
                    for (int i = 0; i < Verticies.Length; i++)
                    {
                        Verticies[i].Color = newColor;
                    }
                }
            }
        }

        public Color Color
        {
            get
            {
                return Verticies.First().Color;
            }

            set
            {
                if(value != Color)
                {
                    for(int i =0; i < Verticies.Length; i++)
                    {
                        Verticies[i].Color = value;
                    }
                }
            }
        }
    }
}
