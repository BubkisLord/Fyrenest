using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fyrenest.Rooms.Abyss
{
    internal class Abyss4 : Room
    {
        public Abyss4() : base("Abyss_04") { }
        public override void OnLoad()
        {
            PlaceGO(Prefabs.SMALL_PLATFORM.Object, 83.5f, 11.8f, null);
            DestroyGO("Direction Pole White Palace (1)");
            PlaceGO(Prefabs.WHITE_PALACE_DIRECTION_POLE.Object, 6.5f, 8.4f, Quaternion.Euler(0, 180, 0));
        }
        public override void OnWorldInit()
        {
            SetTransition("Abyss_04", "right1", "Abyss_18", "right1");
            SetTransition("Abyss_04", "left1", "Abyss_05", "left1");
            SetTransition("Abyss_05", "right1", "Hive_02", "left1");

            SetTransition("Abyss_22", "left1", "Hive_01", "left1");
        }
    }
}
