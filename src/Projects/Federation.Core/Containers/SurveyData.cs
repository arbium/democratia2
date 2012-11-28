using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public struct SurveyData 
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDraft { get; set; }
        public string Tags { get; set; }
        public Guid GroupId { get; set; }
        public short? Duration { get; set; }
        public byte VariantsCount { get; set; }
        public Guid Id { get; set; }
        public IList<SurveyOptionData> Options { get; set; }
        public bool HasOpenProtocol { get; set; }
    }

    public struct SurveyOptionData
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}