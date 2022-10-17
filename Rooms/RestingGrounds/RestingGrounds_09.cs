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

namespace Fyrenest.Rooms.Crossroads
{
    internal class RestingGrounds_09 : Room
    {
        public RestingGrounds_09() : base("RestingGrounds_09") { }

        public override void OnBeforeLoad()
        {
            SetTransition("RestingGrounds_05", "right1", "GG_Workshop", "left1");
            SetTransition("GG_Workshop", "left1", "RestingGrounds_05", "right1");
            SetTransition("RestingGrounds_05", "right2", "GG_Spa", "left1");
        }
    }
}
