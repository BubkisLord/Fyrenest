using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using Satchel.BetterMenus;

namespace Fyrenest
{
    /// <summary>
    /// Handles the "Fyrenest" game mode menu
    /// </summary>
    internal class FyrenestModeMenu : ModeMenuConstructor
    {
        MenuPage SteelSoulSelector;
        private ToggleButton steelSoulToggle;

        /// <summary>
        /// Registers the menu with the ModeMenu
        /// </summary>
        public static void Register()
        {
            ModeMenu.AddMode(new FyrenestModeMenu());
        }

        /// <summary>
        /// Set up the menu page
        /// </summary>
        public override void OnEnterMainMenu(MenuPage modeMenu)
        {
            SteelSoulSelector = new MenuPage("Fyrenest", modeMenu);

            steelSoulToggle = new ToggleButton(SteelSoulSelector, "Steel Soul Mode");

            var mainLabel = new MenuLabel(SteelSoulSelector, "Fyrenest");


            var mapLabel = new MenuLabel(SteelSoulSelector, "Sorry this menu is so bad.", MenuLabel.Style.Body);
            var ideasLabel = new MenuLabel(SteelSoulSelector, "Give ideas for new charms and new area layouts on the HK Modding Discord Server.\nSuggestions are always welcome!", MenuLabel.Style.Body);

            var startButton = new BigButton(SteelSoulSelector, "Start", "Start the game");
            startButton.OnClick += StartGame;

            steelSoulToggle.SetNeighbor(Neighbor.Down, startButton);

            new VerticalItemPanel(SteelSoulSelector, new UnityEngine.Vector2(0, 300), 150, false, new IMenuElement[]
            {
                mainLabel,
                steelSoulToggle,
                mapLabel,
                ideasLabel,
                startButton
            });

            mapLabel.Translate(new UnityEngine.Vector2(250, 0));
            ideasLabel.Translate(new UnityEngine.Vector2(480, 50));
            startButton.Translate(new UnityEngine.Vector2(-91, 75));
        }

        /// <summary>
        /// Enable the mod, and start a new game
        /// </summary>
        public void StartGame()
        {
            Fyrenest.instance.SetEnabled(true);
            UIManager.instance.StartNewGame(permaDeath: steelSoulToggle.Value);
        }

        /// <summary>
        /// Clear the menu instance if the menu is left
        /// </summary>
        public override void OnExitMainMenu()
        {
            SteelSoulSelector = null;
        }

        /// <summary>
        /// Initialize the "Fyrenest" game mode button
        /// </summary>
        public override bool TryGetModeButton(MenuPage modeMenu, out BigButton button)
        {
            button = new BigButton(modeMenu, "Fyrenest", "Adds new charms and powers.");
            button.AddHideAndShowEvent(modeMenu, SteelSoulSelector);
            return true;
        }
    }
}
