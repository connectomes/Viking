﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnotationVizLib.Interfaces
{
    public interface IAttributes
    {
        IDictionary<string, object> Tags { get; }
    }
}
