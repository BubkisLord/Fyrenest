using Modding;

namespace CharmMod
{
    internal class TripleJump : Charm
    {
        public static readonly TripleJump Instance = new();
        public override string Sprite => "SturdyNail.png";
        public override string Name => "Triple Jump";
        public override string Description => "Desc";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private TripleJump() {}

        public override CharmSettings Settings(SaveSettings s) => s.TripleJump;

        public override void Hook()
        {
            
        }

        public int GainSoul(int amount)
        {
            if(Equipped()) {
                if (PlayerData.instance.health == 1) {
                    HeroController.instance.TakeGeo(20);
                    return amount * 2;
                }
                else {
                    HeroController.instance.TakeGeo(10);
                    return amount;
                }
            }
            else {
                return amount;
            }
        }

        //healing costs 100 geo.
        public int BeforeAddHealth( int amount ) {
            if(Equipped()) {
                if(PlayerData.instance.geo > 100 ) {
                    HeroController.instance.AddGeo(100);
                    return amount;
                }
                else
                    return 0;
            }
            else
                return amount;
        }
    }
}