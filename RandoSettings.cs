namespace Fyrenest
{
    public class RandoSettings
    {
        public bool AddCharms;

        [MenuChanger.Attributes.MenuRange(0, 15)]
        public int IncreaseMaxCharmCostBy;

        public static RandoSettings Instance = new RandoSettings();

        public LogicSettings Logic = new();

        public RandoSettings() { }

        public RandoSettings(GlobalSettings rs)
        {
            AddCharms = rs.AddCharms;
            IncreaseMaxCharmCostBy = rs.IncreaseMaxCharmCostBy;
        }

        public bool Enabled() => AddCharms;

        internal void WriteTo(GlobalSettings gs)
        {
            gs.AddCharms = AddCharms;
            gs.IncreaseMaxCharmCostBy = IncreaseMaxCharmCostBy;
        }
    }
}