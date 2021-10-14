using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeWarpEventRandomizer.Warps
{
    internal interface IWarp
    {
        string GlobalWarpId { get; set; }
        int MapWarpId { get; set; }
        string MapId { get; set; }
        int LocationX { get; set; }
        int LocationY { get; set; }
        int LocationElevation { get; set; }
        string DestinationMap { get; set; }
        int DestinationWarpId { get; set; }
    }
}
