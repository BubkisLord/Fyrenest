using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.CityOfTears
{
    internal class City1_18 : Room
    {
        public City1_18() : base("Ruins1_18") { }

        public override void OnLoad()
        {
            DestroyGO("Ruins Lever");
            DestroyGO("Ruins Gate");
        }
    }
}
