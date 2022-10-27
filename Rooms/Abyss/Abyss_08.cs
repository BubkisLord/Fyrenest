using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.Abyss
{
    internal class Abyss_08 : Room
    {
        public Abyss_08() : base("Abyss_08") { }

        public override void OnBeforeLoad()
        {
            SetDarkness(true);
            SetColor(Color.black);
            SetHeroLightColor(Color.black);
        }
    }
}
