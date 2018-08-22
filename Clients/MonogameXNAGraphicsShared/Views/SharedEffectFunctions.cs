﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingXNAGraphics
{
    public static class SharedEffectFunctions
    {
        public static float GetFadeFactor(double ratio, double minCutoff, double maxCuttoff)
        {
            if (ratio < minCutoff)
                return 1f;

            if (ratio > maxCuttoff)
                return 0f;

            return (float)(1.0 - Math.Sqrt((ratio - minCutoff) / (maxCuttoff - minCutoff)));

        }
    }
}
