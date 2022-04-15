using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Shared
{
    public class AddSettingReq
    {
        public string Line { get; set; }
        public int GoalCount { get; set; }
        public ShiftTypeEnum ShiftType { get; set; }
    }
}
