//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConnectomeDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class StructureSpatialView
    {
        public long ID { get; private set; }
        public long TypeID { get; set; }
        public Nullable<long> ParentID { get; set; }
        public double Area { get; set; }
        public double Volume { get; set; }
        public System.Data.Entity.Spatial.DbGeometry ConvexHull { get; set; }
        public System.Data.Entity.Spatial.DbGeometry BoundingBox { get; set; }
        public Nullable<long> MinZ { get; set; }
        public Nullable<long> MaxZ { get; set; }
    }
}
