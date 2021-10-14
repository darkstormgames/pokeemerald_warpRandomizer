using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeWarpEventRandomizer.MapObjects
{
    internal class ObjectEvent
    {
        #region JSON-Members
        public string graphics_id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int elevation { get; set; }
        public string movement_type { get; set; }
        public int movement_range_x { get; set; }
        public int movement_range_y { get; set; }
        public string trainer_type { get; set; }
        public string trainer_sight_or_berry_tree_id { get; set; }
        public string script { get; set; }
        public string flag { get; set; }
        #endregion


    }
}
