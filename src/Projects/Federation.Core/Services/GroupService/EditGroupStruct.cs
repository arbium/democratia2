using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public class EditGroupStruct
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public Guid ParentGroupId { get; set; }
        public string Label { get; set; }
        public string Summary { get; set; }
        public IList<Guid> Categories { get; set; }
        public short ModeratorsCount { get; set; }
        public short ElectionFrequency { get; set; }
        public byte PollQuorum { get; set; }
        public byte ElectionQuorum { get; set; }     
    }
}