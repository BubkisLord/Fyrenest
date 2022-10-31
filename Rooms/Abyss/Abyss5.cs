using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fyrenest.Rooms.Abyss
{
    internal class Abyss5 : Room
    {
        public Abyss5() : base("Abyss_05") { IsFlipped = true; }
        public override void OnBeforeLoad()
        {
            IsFlipped = true;   
        }
        public override void OnLoad()
        {
            DestroyGO("Dusk Knight");
            PlaceGO(Prefabs.PURE_VESSEL_TOTEM.Object, 121.61f, 18.76f, Quaternion.Euler(0, 0, -2.5f));
            PlaceGO(Prefabs.PURE_VESSEL_STATUE.Object, 124.61f, 17, Quaternion.Euler(0, 0, 2.5f));
            PlaceGO(Prefabs.PURE_VESSEL_TOTEM.Object, 127.61f, 18.85f, Quaternion.Euler(0, 0, 1.5f));
        }
    }
}
