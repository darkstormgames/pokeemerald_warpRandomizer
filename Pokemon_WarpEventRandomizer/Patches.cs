using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeWarpEventRandomizer
{
    internal class Patches
    {
        // Game Patches
        public bool EnableTeleport { get; set; }
        public bool DisableEliteWalking { get; set; }
        public bool ReceiveWaterfall { get; set; }
        public bool ReceiveAllHM { get; set; }

        // Gym Options
        public bool RandomizeLavaridge { get; set; }
        public bool DisablePetalburg { get; set; }
        public bool RandomizeMossdeep { get; set; }
        public bool KeepSootopolisLadder { get; set; }

        // Event/Map Options
        public bool RemoveDevonGuard { get; set; }
        public bool DisableGymWalk { get; set; }
        public bool ConnectSootopolis { get; set; }
    }
}
