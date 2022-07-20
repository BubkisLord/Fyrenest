using Modding;

namespace Fyrenest
{
    internal class FyreChild : Charm
    {
        public static readonly FyreChild Instance = new();
        public override string Sprite => "FyreChild.png";
        public override string Name => "Grimm's Flame";
        public override string Description => "A fiery charm radiating strength. When worn, makes the bearer's Grimmchild more powerful.\n\nYour Grimmchild does more damage, is faster, and shoots quicker. These effects get more powerful as you attack more enemies.";
        public override int DefaultCost => 3;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private FyreChild() {}

        public override CharmSettings Settings(SaveSettings s) => s.FyreChild;

        //refer to other mod, and look at void soul code for hitting enemies to make the abilities stronger.
}