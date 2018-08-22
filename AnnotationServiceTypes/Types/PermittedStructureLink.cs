﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using ProtoBuf; 

namespace AnnotationService.Types
{
    [ProtoContract]
    [DataContract]
    public class PermittedStructureLink : DataObject
    {
        public override string ToString()
        {
            string result = _SourceTypeID.ToString();
            result += _Bidirectional ? " <-> " : " -> ";
            result += _TargetTypeID.ToString();
            return result;
        }

        Int64 _SourceTypeID;
        Int64 _TargetTypeID;
        bool _Bidirectional;

        [DataMember]
        [ProtoMember(1)]
        public Int64 SourceTypeID
        {
            get { return _SourceTypeID; }
            set { _SourceTypeID = value; }
        }

        [DataMember]
        [ProtoMember(2)]
        public Int64 TargetTypeID
        {
            get { return _TargetTypeID; }
            set { _TargetTypeID = value; }
        }

        [DataMember]
        [ProtoMember(3)]
        public bool Bidirectional
        {
            get { return _Bidirectional; }
            set { _Bidirectional = value; }
        }

        public PermittedStructureLink()
        {
        }

        
    }
}
