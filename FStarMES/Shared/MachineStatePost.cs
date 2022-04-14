using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Shared
{
    public class MachineStatePost
    {
        public string Line { get; set; }
        public string Station { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public StateStatusEnum StateStatus { get; set; }
        public string? StateDetail { get; set; }
        public string ProductType { get; set; }
    }
}
