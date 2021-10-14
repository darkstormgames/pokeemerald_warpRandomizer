using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeWarpEventRandomizer.Warps
{
    internal class Warp : IWarp
    {
        public string GlobalWarpId { get; set; } = Guid.NewGuid().ToString();
        public int MapWarpId { get; set; }
        public string MapId { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public int LocationElevation { get; set; }
        public string DestinationMap { get; set; }
        public int DestinationWarpId { get; set; }

        public bool IsLocked { get; set; }
        public bool IsTarget { get; set; }
        public string TargetWarpGlobalId { get; set; }
        public List<string> NeighborWarpsGlobalId { get; set; } = new List<string>();

        [JsonIgnore]
        public Warp TargetWarp => FullWarpList.FirstOrDefault(p => !string.IsNullOrEmpty(this.TargetWarpGlobalId) && p.GlobalWarpId == this.TargetWarpGlobalId);

        [JsonIgnore]
        public bool HasNeighbor => NeighborWarps != null && NeighborWarps.Count > 0;
        [JsonIgnore]
        public List<Warp> NeighborWarps 
        { 
            get
            {
                if (this.FullWarpList == null)
                {
                    return null;
                }
                else
                {
                    List<Warp> result = new List<Warp>();
                    foreach (string item in this.NeighborWarpsGlobalId)
                    {
                        Warp warp = FullWarpList.FirstOrDefault(p => p.GlobalWarpId == item);
                        if (warp != null)
                        {
                            result.Add(warp);
                        }
                    }
                    return result;
                }
            }
        }

        [JsonIgnore]
        public List<Warp> FullWarpList { get; set; }


        public void SetFullList(List<Warp> warps)
        {
            this.FullWarpList = warps;
        }
    }
}
