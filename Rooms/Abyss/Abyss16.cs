using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.Abyss
{
    internal class Abyss16 : Room
    {
        public Abyss16() : base("Abyss_16") { }

        public override void OnBeforeLoad()
        {
            SetDarkness(true);
        }
    }
}
