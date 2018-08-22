﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VikingXNAGraphics
{
    /// <summary>
    /// An interface for views that have a color which can be adjusted
    /// </summary>
    public interface IColorView
    {
        Color Color { get; set; }
        float Alpha { get; set; }
    }
}
