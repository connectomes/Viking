﻿using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AnnotationVizLib;

namespace AnnotationVizLibTests
{
    [TestClass]
    public class ColorMappingTest
    {
        public static bool ColorsMatch(Color A, Color B)
        {
            return A.R == B.R &&
                   A.G == B.G &&
                   A.B == B.B &&
                   A.A == B.A; 
        }
        /// <summary>
        /// Tests that pixels can be read from a colormap image correctly.  The colormap image is 3 columns by 2 rows.
        /// Black Red Green
        /// White Blue Gray128
        /// </summary>
        [TestMethod]
        public void TestColorMapImage()
        {
           
            Scale scale = new Scale(new AxisUnits(1.0, "nm"),
                                    new AxisUnits(1.0, "nm"),
                                    new AxisUnits(1.0, "nm"));
            ColorMapImageData colormap = null;
            using(System.IO.Stream imagestream = System.IO.File.OpenRead("Resources\\ColorMap3x2.png"))
            {
                colormap = new ColorMapImageData(imagestream, scale);
            }

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, 0), Color.Black));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, 1), Color.White));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(1, 0), Color.FromArgb(255, 0,0)));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(2, 0), Color.FromArgb(0, 255, 0)));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(1, 1), Color.FromArgb(0,0,255)));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(2, 1), Color.FromArgb(128,128,128)));

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(3, 0), Color.Empty));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, 2), Color.Empty));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(-1, 0), Color.Empty));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, -1), Color.Empty));
        }


        /// <summary>
        /// Same as TestColorMapImage, but the input image is scaled by a factor of four.
        /// </summary>
        [TestMethod]
        public void TestScaledColorMapImage()
        {

            Scale scale = new Scale(new AxisUnits(4.0, "nm"),
                                    new AxisUnits(4.0, "nm"),
                                    new AxisUnits(4.0, "nm"));
            ColorMapImageData colormap = null;
            using (System.IO.Stream imagestream = System.IO.File.OpenRead("Resources\\ColorMap12x8.png"))
            {
                colormap = new ColorMapImageData(imagestream, scale);
            }

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, 0), Color.Black));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, 1), Color.White));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(1, 0), Color.FromArgb(255, 0, 0)));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(2, 0), Color.FromArgb(0, 255, 0)));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(1, 1), Color.FromArgb(0, 0, 255)));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(2, 1), Color.FromArgb(128, 128, 128)));

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(3, 0), Color.Empty));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, 2), Color.Empty));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(-1, 0), Color.Empty));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ColorsMatch(colormap.GetColor(0, -1), Color.Empty));
        }
    }
}