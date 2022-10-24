using ItemChanger;
using ItemChanger.Components;
using ItemChanger.Internal;
using ItemChanger.Locations;
using ItemChanger.Locations.SpecialLocations;
using ItemChanger.Placements;
using ItemChanger.UIDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.Deepnest
{
    internal class Deepnest_9 : Room
    {
        public Deepnest_9() : base("Deepnest_09") { IsFlipped = true; }
        public override void OnBeforeLoad()
        {
            SetColor(Color.magenta);
            //SetSaturation(100f);
        }
    }
}
