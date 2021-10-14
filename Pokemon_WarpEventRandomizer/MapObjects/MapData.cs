using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PokeWarpEventRandomizer.MapObjects;

namespace PokeWarpEventRandomizer
{
    internal class MapData
    {
        #region JSON-Members
        public string id { get; set; }
        public string name {  get; set; }
        public string layout { get; set; }
        public string music { get; set; }
        public string region_map_section { get; set; }
        public bool requires_flash { get; set; }
        public string weather { get; set; }
        public string map_type { get; set; }
        public bool allow_cycling { get; set; }
        public bool allow_escaping { get; set; }
        public bool allow_running { get; set; }
        public bool show_map_name { get; set; }
        public string battle_scene { get; set; }
        public List<Connection> connections { get; set; }
        public List<ObjectEvent> object_events { get; set; }
        public List<WarpEvent> warp_events { get; set; }
        public List<CoordEvent> coord_events { get; set; }
        public List<BGEvent> bg_events { get; set; }
        #endregion


    }
}
