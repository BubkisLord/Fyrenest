using Modding;
using UnityEngine;
using GlobalEnums;

namespace Fyrenest
{
    internal class Slowfall : Charm
    {
        public static readonly Slowfall Instance = new();
        public override string Sprite => "Slowfall.png";
        public override string Name => "Slowfall";
        public override string Description => "This charm falls slower than it should when dropped.\n\nWhen worn, the bearer falls at a very slow rate.";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private Slowfall() {}

        public override CharmSettings Settings(SaveSettings s) => s.Slowfall;
    }
}