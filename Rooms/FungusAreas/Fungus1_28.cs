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
using TMPro;
using UnityEngine;

namespace Fyrenest.Rooms.FungalWastes
{
    internal class Fungus1_28 : Room
    {
        //repurposed final boss room
        public Fungus1_28() : base("Fungus1_28") { }
        public override void OnBeforeLoad()
        {
            PlaceTransition("Fungus1_28", "left2", "RestingGrounds_07", "right1", 10, 10, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default, true);
            SetTransition("Fungus1_28", "left2", "RestingGrounds_07", "right1");
        }
        public override void OnLoad()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Terrain"))
            {
                GameObject.Destroy(go);
            }
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 1, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 4, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 7, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 10, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 13, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 16, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 19, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 22, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 25, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 28, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 31, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 34, 7);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 37, 7);
        }
        public override void OnWorldInit()
        {
            PlaceTransition("Fungus1_28", "left2", "RestingGrounds_07", "right1", 10, 10, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default, true);
            //SetTransition("RestingGrounds_07", "right1", "Fungus1_28", "left2");
        }
    }
}
