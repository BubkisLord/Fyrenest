using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fyrenest.Rooms.CityOfTears
{
    internal class GG_Waterways : Room
    {
        public GG_Waterways() : base("GG_Waterways") { }

        public override void OnLoad()
        {
            DestroyGO("Dream Enter");
        }
        public override void OnBeforeLoad()
        {
            DestroyGO("Dream Enter");
        }
    }
}
