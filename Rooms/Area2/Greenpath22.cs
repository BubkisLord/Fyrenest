using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.Area2
{
    internal class Greenpath22 : Room
    {
        public static string NAME = "Fungus1_22";

        public Greenpath22() : base(NAME) { }

        public override void OnLoad()
        {
            DestroyGO("Gate Switch");
            DestroyGO("Metal Gate");
        }
    }
}
