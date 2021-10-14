using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeWarpEventRandomizer.MapObjects
{
    internal class CoordEvent
    {
        #region JSON-Members
        public string type { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int elevation { get; set; }
        public string var { get; set; }
        public string var_value { get; set; }
        public string script { get; set; }
        public string weather { get; set; }
        #endregion


    }
}
