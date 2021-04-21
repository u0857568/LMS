using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public DateTime DateTime { get; set; }
        public uint Score { get; set; }
        public string Contents { get; set; }
        public string UId { get; set; }
        public int Aid { get; set; }

        public virtual Assignments A { get; set; }
        public virtual Students U { get; set; }
    }
}
