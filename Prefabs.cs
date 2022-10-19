using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fyrenest
{
    /// <summary>
    /// Contains the data necessary to find and load a prefab
    /// </summary>
    class Prefab
    {
        public string OriginRoom;
        public string OriginName;

        public GameObject Object;

        public Prefab(string originRoom, string originName)
        {
            OriginRoom = originRoom;
            OriginName = originName;
        }
    }

    /// <summary>
    /// Contains the prefabs to be loaded by the PrefabManager
    /// </summary>
    internal class Prefabs
    {
        public static Prefab BREAKABLE_FLOOR = new("RestingGrounds_05", "Quake Floor");
        public static Prefab BREAKABLE_WALL = new("Crossroads_07", "Breakable Wall_Silhouette");
        public static Prefab LEFT_TRANSITION = new("Crossroads_01", "left1");
        public static Prefab RIGHT_TRANSITION = new("Crossroads_01", "_Transition Gates/right1");
        public static Prefab TOP_TRANSITION = new("Crossroads_01", "_Transition Gates/top1");
        public static Prefab SMALL_PLATFORM = new("Tutorial_01", "_Scenery/plat_float_17");
        public static Prefab LARGE_PLATFORM = new("Crossroads_01", "_Scenery/plat_float_07");
        public static Prefab GARDENS_PLATFORM = new("Fungus3_08", "Royal Gardens Plat S");
        public static Prefab BOUNCE_MUSHROOM = new("Fungus2_20", "Bounce Shroom B");
        public static Prefab DUSK_KNIGHT = new("Abyss_05", "Dusk Knight");
        public static Prefab WHITE_PALACE_ENTRANCE = new("White_Palace_11", "door1");
        public static Prefab DREAM_ENTRY = new("White_Palace_11", "Dream Entry");
        public static Prefab WHITE_PALACE_LEVER = new("White_Palace_14", "White Palace Orb Lever");
        public static Prefab PURE_VESSEL_STATUE = new("GG_Workshop", "GG_Statue_HollowKnight");
        public static Prefab WHITE_PALACE_DIRECTION_POLE = new("Abyss_04", "Direction Pole White Palace (1)");
        public static Prefab GEO_ROCK_GREENPATH = new("Fungus3_10", "Geo Rock Green Path 01");
        public static Prefab GEO_ROCK = new("Crossroads_07", "Geo Rock 1 (1)");
        public static Prefab PURE_VESSEL_TOTEM = new("White_Palace_20", "Soul Totem white_Infinte (2)");
        public static Prefab PANTHEON_V = new("GG_Atrium_Roof", "GG_Final_Challenge_Door");
    }
}
