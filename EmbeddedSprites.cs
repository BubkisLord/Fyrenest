namespace Fyrenest
{
    internal static class EmbeddedSprite
    {
        private static Dictionary<string, Sprite> Sprites = new();

        public static Sprite Get(string name)
        {
            if (Sprites.TryGetValue(name, out var sprite))
            {
                return sprite;
            }
            sprite = LoadSprite(name);
            Sprites[name] = sprite;
            return sprite;
        }

        private static Sprite LoadSprite(string name)
        {
            var loc = Path.Combine(Path.GetDirectoryName(typeof(EmbeddedSprite).Assembly.Location), name);
            var imageData = File.ReadAllBytes(loc);
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(tex, imageData, true);
            tex.filterMode = FilterMode.Bilinear;
            if (loc.Contains("GlassCannon.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.2f);
            else if (loc.Contains("SoulHunger.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.73f);
            else if (loc.Contains("RavenousSoul.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.73f);
            else if (loc.Contains("SoulSwitch.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.25f);
            else if (loc.Contains("WealthyAmulet.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.62f);
            else if (loc.Contains("SoulSlow.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 1.05f);
            else if (loc.Contains("Fyrechild.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.25f);
            else if (loc.Contains("WyrmForm.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 1.1f);
            else if (loc.Contains("ElderStone.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.75f);
            else if (loc.Contains("Fyrechild.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.35f);
            else if (loc.Contains("LifeBloodCharm.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 1.85f);
            else if (loc.Contains("SoulSpeed.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.2f);
            else if (loc.Contains("SturdyNail.png")) return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f / 0.5f);
            else return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
        }
    }
}