using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Shared
{
    public class UpdateSettingReq
    {
        public int Id { get; set; }
        [Required]
        public int GoalCount { get; set; }
    }
}
