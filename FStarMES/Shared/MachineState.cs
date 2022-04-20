using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Shared
{
    public class MachineState
    {
        public long Id { get; set; }
        public string Line { get; set; }
        public string Station { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public StateStatusEnum StateStatus { get; set; }
        public string StateDetail { get; set; }
        public string ProductType { get; set; }
        public DateTime CreationTime { get; set; }
    }
    public enum StateStatusEnum
    {
        Run = 1,
        Wait = 2,
        Error = 3
    }
}
