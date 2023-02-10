using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms
{
    internal class Transitions : Room
    {
        public Transitions() : base("Town") { }

        public override void OnLoad()
        {
        }

        public override void OnWorldInit()
        {
            SetTransition("Fungus3_47", "right1", "Deepnest_40", "right1");

        }
    }
    internal class Deepnest_40 : Room
    {
        public Deepnest_40() : base("Deepnest_40") { IsFlipped = true; }
    }
}
