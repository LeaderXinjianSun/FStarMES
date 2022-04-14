using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Shared
{
    public class LineSeting
    {
        public int Id { get; set; }
        public string Line { get; set; }
        public int GoalCount { get; set; }
        public ShiftTypeEnum ShiftType { get; set; }
    }
    public enum ShiftTypeEnum
    {
        Day = 1,
        Night = 2
    }
}
