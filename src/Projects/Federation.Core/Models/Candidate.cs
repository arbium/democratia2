using System;

namespace Federation.Core
{
    public partial class Candidate
    {
        public Candidate()
        {
            Id = Guid.NewGuid();
        }
    }
}