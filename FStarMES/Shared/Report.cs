using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FStarMES.Shared
{
    public class Report
    {
        /// <summary>
        /// 线别
        /// </summary>
        [Required]
        public string? Line { get; set; }
        /// <summary>
        /// 生产日期：4/5
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        [Required]
        public int Month { get; set; }
        /// <summary>
        /// 班别
        /// </summary>
        [Required]
        public ShiftTypeEnum ShiftType { get; set; }
        /// <summary>
        /// 产品：包装
        /// </summary>
        [Required]
        public string? ProductType { get; set; }
        /// <summary>
        /// 总产出量
        /// </summary>
        [Required]
        public int Count { get; set; }
        /// <summary>
        /// 目标值
        /// </summary>
        [Required]
        public int GoalCount { get; set; }
        /// <summary>
        /// 完成率
        /// </summary>
        [Required]
        public double FillRate { get; set; }
        /// <summary>
        /// 未完成率
        /// </summary>
        [Required]
        public double NotReachRate { get; set; }
        /// <summary>
        /// 停产时间
        /// </summary>
        [Required]
        public string? HaltRange { get; set; }
        /// <summary>
        /// 停产分类
        /// </summary>
        [Required]
        public string? HaltType { get; set; }
        /// <summary>
        /// 停产原因
        /// </summary>
        [Required]
        public string? HaltReason { get; set; }
        /// <summary>
        /// 停产总时间
        /// </summary>
        [Required]
        public string? HaltElapse { get; set; }
    }
}
