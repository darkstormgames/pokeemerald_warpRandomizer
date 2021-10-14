using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeWarpEventRandomizer.MapObjects
{
    internal class WarpEvent
    {
        #region JSON-Members
        public int x { get; set; }
        public int y { get; set; }
        public int elevation { get; set; }
        public string dest_map { get; set; }
        public int dest_warp_id { get; set; }
        #endregion


    }
}
