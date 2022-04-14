using FStarMES.Shared;
using ShardingCore.VirtualRoutes.Months;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarMES.Data
{
    public class MachineStateVirtualTableRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<MachineState>
    {
        public override bool AutoCreateTableByTime()
        {
            return true;
        }

        public override void Configure(ShardingCore.Core.EntityMetadatas.EntityMetadataTableBuilder<MachineState> builder)
        {
            builder.ShardingProperty(o => o.CreationTime);
        }

        public override DateTime GetBeginTime()
        {
            return new DateTime(2022, 4, 1);
        }
    }
}
