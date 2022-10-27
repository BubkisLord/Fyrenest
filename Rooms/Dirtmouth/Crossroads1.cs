namespace Fyrenest.Rooms.Crossroads
{
    internal class Crossroads1 : Room
    {
        public Crossroads1() : base("Crossroads_01") { }

        public override void OnBeforeLoad()
        {
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 37.2f, 6f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 37.2f, 7f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 37.2f, 8f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 38.2f, 6f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 38.2f, 7f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 38.2f, 8f);
        }
        public override void OnWorldInit()
        {
            SetItem(LocationNames.Geo_Rock_Crossroads_Well, ItemNames.Charm_Notch);
        }
    }
}
