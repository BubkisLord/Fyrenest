using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.CityOfTears
{
    internal class City : Room
    {
        public City() : base("City") { }

        public override void OnWorldInit()
        {
            SetItem(LocationNames.Iselda, SoulHunger.instance.Name, true, 1200, alternateName: "Wierd Rock", alternateDesc: "My husband Conifer found this strange rock next to a statue of what apparently looked to be a larger version of you. It looks precious, so I'm putting it at quite a price.");
        }
    }
}
