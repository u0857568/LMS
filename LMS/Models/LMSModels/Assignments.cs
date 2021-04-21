using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submission = new HashSet<Submission>();
        }

        public int Aid { get; set; }
        public string Name { get; set; }
        public uint MaxPointValue { get; set; }
        public string Contents { get; set; }
        public DateTime Due { get; set; }
        public uint Acid { get; set; }

        public virtual AssignmentCategories Ac { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
