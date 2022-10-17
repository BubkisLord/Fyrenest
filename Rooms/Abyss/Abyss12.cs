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

namespace Fyrenest.Rooms.Abyss
{
    internal class Abyss12 : Room
    {
        public Abyss12() : base("Abyss_12") { }

        public override void OnBeforeLoad()
        {
            SetDarkness(true);
        }
    }
}
