using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FStarMES.Shared
{
    public class LineSeting
    {
        public int Id { get; set; }
        [Required]
        public string? Line { get; set; }
        [Required]
        public int GoalCount { get; set; }
        [Required]
        public ShiftTypeEnum ShiftType { get; set; }
    }
    public enum ShiftTypeEnum
    {
        Day = 1,
        Night = 2
    }
}
