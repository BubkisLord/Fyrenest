namespace Fyrenest.Rooms.KingdomsEdge
{
    internal class KingdomsEdge : Room
    {
        public KingdomsEdge() : base("Deepnest_East_01") { }
        public override void OnWorldInit()
        {
            // replace god tuner with void soul
            AbstractPlacement placement = Finder.GetLocation(LocationNames.Godtuner).Wrap();
            AbstractItem aitem = new VoidSoul();
            aitem.OnGive += OnGivenVoidSoul;
            aitem.UIDef = new MsgUIDef()
            {
                name = new BoxedString("VoidSoul"),
                sprite = new BoxedSprite(EmbeddedSprite.Get("VoidSoul.png")),
            };
            placement.Add(aitem);
            ItemChangerMod.AddPlacements(new AbstractPlacement[] { placement }, PlacementConflictResolution.MergeKeepingNew);
        }
        private void OnGivenVoidSoul(ReadOnlyGiveEventArgs obj)
        {
            VoidSoul.instance.SetObtained();
            VoidSoul.instance.Settings(Fyrenest.instance.Settings).Got = true;
        }
    }
}
