//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConnectomeODataV4.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location_CT
    {
        public byte[] C___start_lsn { get; set; }
        public byte[] C___end_lsn { get; set; }
        public byte[] C___seqval { get; set; }
        public int C___operation { get; set; }
        public byte[] C___update_mask { get; set; }
        public Nullable<long> ID { get; set; }
        public Nullable<long> ParentID { get; set; }
        public Nullable<double> X { get; set; }
        public Nullable<double> Y { get; set; }
        public Nullable<double> Z { get; set; }
        public byte[] Verticies { get; set; }
        public Nullable<bool> Closed { get; set; }
        public byte[] Version { get; set; }
        public byte[] Overlay { get; set; }
        public string Tags { get; set; }
        public Nullable<double> VolumeX { get; set; }
        public Nullable<double> VolumeY { get; set; }
        public Nullable<bool> Terminal { get; set; }
        public Nullable<bool> OffEdge { get; set; }
        public Nullable<double> Radius { get; set; }
        public Nullable<short> TypeCode { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string Username { get; set; }
    }
}
