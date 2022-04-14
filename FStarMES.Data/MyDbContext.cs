using FStarMES.Shared;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Data
{
    public class MyDbContext : AbstractShardingDbContext, IShardingTableDbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MachineState>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd().HasComment("状态Id");
                entity.Property(o => o.Line).IsRequired().IsUnicode(false).HasMaxLength(50).HasComment("线体");
                entity.Property(o => o.Station).IsRequired().IsUnicode(false).HasMaxLength(50).HasComment("站点");
                entity.Property(o => o.Count).IsRequired().HasComment("产量");
                entity.Property(o => o.TotalCount).IsRequired().HasComment("总产量");
                entity.Property(o => o.StateDetail).IsRequired(false).IsUnicode(false).HasMaxLength(50).HasComment("报警明细");
                entity.Property(o => o.StateStatus).IsRequired().HasConversion<int>().HasComment("状态");
                entity.Property(o => o.ProductType).IsRequired().IsUnicode(false).HasMaxLength(50).HasComment("产品");
                entity.ToTable(nameof(MachineState));
            });
            modelBuilder.Entity<LineSeting>(entity => {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd().HasComment("设置Id");
                entity.Property(o => o.Line).IsRequired().IsUnicode(false).HasMaxLength(50).HasComment("线体");
                entity.Property(o => o.GoalCount).IsRequired().HasComment("目标产量");
                entity.Property(o => o.ShiftType).IsRequired().HasConversion<int>().HasComment("班别");
            });
        }
        public IRouteTail RouteTail { get; set; }
    }
}
