using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Fyrenest.Consts
{
    public class TextureStrings
    {
        #region Misc
        public const string QuickfallKey = "Quickfall";
        private const string QuickfallFile = "Fyrenest.Resources.Quickfall.png";
        public const string SlowfallKey = "Slowfall";
        private const string SlowfallFile = "Fyrenest.Resources.Slowfall.png";
        public const string SturdyNailKey = "SturdyNail";
        private const string SturdyNailFile = "Fyrenest.Resources.SturdyNail.png";
        public const string BetterCDashKey = "BetterCDash";
        private const string BetterCDashFile = "Fyrenest.Resources.BetterCDash.png";
        public const string GlassCannonKey = "GlassCannon";
        private const string GlassCannonFile = "Fyrenest.Resources.GlassCannon.png";
        public const string HKBlessingKey = "HKBlessing";
        private const string HKBlessingFile = "Fyrenest.Resources.HKBlessing.png";
        public const string HuntersMarkKey = "HuntersMark";
        private const string HuntersMarkFile = "Fyrenest.Resources.HuntersMark.png";
        public const string PowerfulDashKey = "PowerfulDash";
        private const string PowerfulDashFile = "Fyrenest.Resources.PowerfulDash.png";
        public const string WealthyAmuletKey = "WealthyAmulet";
        private const string WealthyAmuletFile = "Fyrenest.Resources.WealthyAmulet.png";
        public const string TripleJumpKey = "TripleJump";
        private const string TripleJumpFile = "Fyrenest.Resources.TripleJump.png";
        #endregion Misc

        private readonly Dictionary<string, Sprite> _dict;

        public TextureStrings()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            _dict = new Dictionary<string, Sprite>();
            Dictionary<string, string> tmpTextures = new Dictionary<string, string>();
            tmpTextures.Add(QuickfallKey, QuickfallFile);
            tmpTextures.Add(SlowfallKey, SlowfallFile);
            tmpTextures.Add(SturdyNailKey, SturdyNailFile);
            tmpTextures.Add(BetterCDashKey, BetterCDashFile);
            tmpTextures.Add(GlassCannonKey, GlassCannonFile);
            tmpTextures.Add(HKBlessingKey, HKBlessingFile);
            tmpTextures.Add(HuntersMarkKey, HuntersMarkFile);
            tmpTextures.Add(PowerfulDashKey, PowerfulDashFile);
            tmpTextures.Add(WealthyAmuletKey, WealthyAmuletFile);
            tmpTextures.Add(TripleJumpKey, TripleJumpFile);
            foreach (var t in tmpTextures)
            {
                using (Stream s = asm.GetManifestResourceStream(t.Value))
                {
                    if (s == null) continue;

                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    // Create sprite from texture
                    // Split is to cut off the TestOfTeamwork.Resources. and the .png
                    _dict.Add(t.Key, Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));
                }
            }
        }

        public Sprite Get(string key)
        {
            return _dict[key];
        }
    }
}