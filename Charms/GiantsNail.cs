using Modding;

namespace Fyrenest
{
    internal class GiantNail : Charm
    {
        public static readonly GiantNail Instance = new();
        public override string Sprite => "GiantNail.png";
        public override string Name => "Giant's Nail";
        public override string Description => "This charm enlarges your nail dramatically.\n\nYou gain an extraordinary amount of power, having the your nail transformed into a giant's.";
        public override int DefaultCost => 4;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private GiantNail() { }

        public override CharmSettings Settings(SaveSettings s) => s.GiantNail;

        public override void Hook()
        {
            ModHooks.SlashHitHook += ChangeNailScale;
        }

        public void ChangeNailScale(Collider2D collider, GameObject slash)
        {
            if (Equipped())
            {
                if (!HeroController.instance.cState.downAttacking)
                {
                    slash.transform.SetScaleY(2);
                    if (PlayerData.instance.equippedCharm_13 && PlayerData.instance.equippedCharm_18) slash.transform.SetScaleX(5);
                    if (!PlayerData.instance.equippedCharm_13 && PlayerData.instance.equippedCharm_18) slash.transform.SetScaleX(4);
                    if (PlayerData.instance.equippedCharm_13 && !PlayerData.instance.equippedCharm_18) slash.transform.SetScaleX(3);
                    if (!PlayerData.instance.equippedCharm_13 && !PlayerData.instance.equippedCharm_18) slash.transform.SetScaleX(2);
                    if (HeroController.instance.cState.facingRight) HeroController.instance.RecoilLeftLong();
                    if (!HeroController.instance.cState.facingRight) HeroController.instance.RecoilRightLong();
                }
                else
                {
                    slash.transform.SetScaleX(2);
                    if (!HeroController.instance.cState.upAttacking)
                    {
                        if (PlayerData.instance.equippedCharm_13 && PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(5);
                        if (!PlayerData.instance.equippedCharm_13 && PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(4);
                        if (PlayerData.instance.equippedCharm_13 && !PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(3);
                        if (!PlayerData.instance.equippedCharm_13 && !PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(2);
                    }
                    else
                    {
                        if (PlayerData.instance.equippedCharm_13 && PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(5);
                        if (!PlayerData.instance.equippedCharm_13 && PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(4);
                        if (PlayerData.instance.equippedCharm_13 && !PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(3);
                        if (!PlayerData.instance.equippedCharm_13 && !PlayerData.instance.equippedCharm_18) slash.transform.SetScaleY(2);
                        float endingYPos = HeroController.instance.transform.GetPositionY() + 0.6f;
                        if (Equipped()) HeroController.instance.transform.SetPositionY(endingYPos);
                    }
                }
            }
            return;
        }
    }
}