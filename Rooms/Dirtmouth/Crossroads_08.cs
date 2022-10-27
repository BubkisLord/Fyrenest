namespace Fyrenest.Rooms.Crossroads
{
    internal class Crossroads_08 : Room
    {
        public Crossroads_08() : base("Crossroads_08") { }
        public override void OnBeforeLoad()
        {
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 50, 20, Quaternion.Euler(0, 0, 270));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 50, 23, Quaternion.Euler(0, 0, 270));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 50, 26, Quaternion.Euler(0, 0, 270));
        }
    }
}
