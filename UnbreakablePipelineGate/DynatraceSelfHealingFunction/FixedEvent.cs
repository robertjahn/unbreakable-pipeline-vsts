using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceSelfHealingFunction
{
    public class FixedEvent
    {
        public dynamic OrigEvent { get; set; }
        public int RollbackReleaseId { get; set; }
        public string Response { get; set; }
    }
}
