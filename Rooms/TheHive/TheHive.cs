using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.TheHive
{
    internal class Hive_01 : Room
    {
        public Hive_01() : base("Hive_01") { IsFlipped = true; }
    }
    internal class Hive_02 : Room
    {
        public Hive_02() : base("Hive_02") { IsFlipped = true; }

        public override void OnWorldInit()
        {
            SetTransition("Hive_01", "left1", "Abyss_22", "left1");
        }
    }
    internal class Hive_03 : Room
    {
        public Hive_03() : base("Hive_03") { IsFlipped = true; }
    }
    internal class Hive_03_c : Room
    {
        public Hive_03_c() : base("Hive_03_c") { IsFlipped = true; }
        public override void OnLoad()
        {
            // In The Hive, to stop u going to Kingdom's Edge.
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 60, 70.5f, Quaternion.Euler(0, 0, 90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 58, 70.5f, Quaternion.Euler(0, 0, 90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 60, 72.5f, Quaternion.Euler(0, 0, 90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 58, 72.5f, Quaternion.Euler(0, 0, 90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 60, 68.5f, Quaternion.Euler(0, 0, 90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 58, 68.5f, Quaternion.Euler(0, 0, 90));

            // At Kingdom's Edge entrance to The Hive (shouldn't even be possible, but just incase)
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 45, 36, Quaternion.Euler(0, 0, 90));
        }
    }
    internal class Hive_04 : Room
    {
        public Hive_04() : base("Hive_04") { IsFlipped = true; }
    }
    internal class Hive_05 : Room
    {
        public Hive_05() : base("Hive_05") { IsFlipped = true; }
    }
}
