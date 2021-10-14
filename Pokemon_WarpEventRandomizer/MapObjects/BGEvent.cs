using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeWarpEventRandomizer.MapObjects
{
    internal class BGEvent
    {
        #region JSON-Members
        public string type { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int elevation { get; set; }
        public string player_facing_dir { get; set; }
        public string script { get; set; }
        public string item { get; set; }
        public string flag { get; set; }
        #endregion


    }
}
