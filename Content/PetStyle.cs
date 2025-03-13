using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace LolaPet.Content
{
    public enum DefaultStyles
    {
        Default = -1, 
        DarknessTrigger = -2
    }

    public class PetStyle : ModSystem
    {
        public static Dictionary<int, (Color[], string, bool, int)> HologramStyle;
        public static Vector2 HologramSize = new Vector2(64, 78);
        public static Dictionary<int, (Color[], Texture2D, Texture2D, bool, int)> ForeignHologramStyles = 
            new Dictionary<int, (Color[], Texture2D, Texture2D, bool, int)>();
        public static float ColorChangeTimer = 0;

        public override void Load()
        {
            HologramStyle = new Dictionary<int, (Color[], string, bool, int)>();
        }

        public override void PostSetupContent()
        {
            HologramStyle[-2] = ([new Color(209, 125, 255)], "DarknessTrigger", false, 5);
            HologramStyle[-1] = ([new Color(253, 26, 114)], "Default", false, 0);
            HologramStyle[ItemID.VampireKnives] = ([new Color(255, 68, 51)], "VampireKnives", false, 0);
            HologramStyle[ItemID.TerraBlade] = ([new Color(15, 84, 125), new Color(34, 177, 76),
                new Color(233, 202, 27)], "Terrablade", false, 0);
            HologramStyle[ItemID.Minishark] = ([new Color(73, 74, 73)], "Minishark", false, 0);
            HologramStyle[ItemID.RazorbladeTyphoon] = ([new Color(48, 248, 171)], "RazorbladeTyphoon", false, 0);
            HologramStyle[ItemID.Tsunami] = ([new Color(48, 248, 171)], "RazorbladeTyphoon", false, 0);
            HologramStyle[ItemID.Flairon] = ([new Color(48, 248, 171)], "RazorbladeTyphoon", false, 0);
            HologramStyle[ItemID.TempestStaff] = ([new Color(48, 248, 171)], "RazorbladeTyphoon", false, 0);
            HologramStyle[ItemID.BubbleGun] = ([new Color(48, 248, 171)], "RazorbladeTyphoon", false, 0);
            HologramStyle[ItemID.Meowmere] = ([new Color(243, 235, 243)], "Meowmere", false, 0);
            HologramStyle[ItemID.DeathSickle] = ([new Color(60, 60, 60)], "DeathSickle", false, 0);
            HologramStyle[ItemID.DemonScythe] = ([new Color(63, 60, 134)], "DemonScythe", false, 0);
            HologramStyle[ItemID.NightsEdge] = ([new Color(139, 42, 156)], "NightsEdge", false, 0);
            HologramStyle[ItemID.TrueNightsEdge] = ([new Color(139, 42, 156)], "TrueNightsEdge", true, 0);
            HologramStyle[ItemID.EnchantedSword] = ([new Color(69, 96, 233)], "EnchantedSword", false, 0);
            HologramStyle[ItemID.IceBlade] = ([new Color(68, 187, 238)], "Frostbrand", false, 0);
            HologramStyle[ItemID.Frostbrand] = ([new Color(68, 187, 238)], "Frostbrand", false, 0);
            HologramStyle[ItemID.ChargedBlasterCannon] = ([new Color(0, 115, 255)], "BlasterCannon", false, 0);
            HologramStyle[ItemID.BeeGun] = ([new Color(254, 194, 20)], "BeeGun", false, 0);
            HologramStyle[ItemID.BeeKeeper] = ([new Color(254, 194, 20)], "BeeGun", false, 0);
            HologramStyle[ItemID.BeesKnees] = ([new Color(254, 194, 20)], "BeeGun", false, 0);
            HologramStyle[ItemID.Beenade] = ([new Color(254, 194, 20)], "BeeGun", false, 0);
            HologramStyle[ItemID.InfernoFork] = ([new Color(230, 40, 40)], "InfernoFork", false, 0);
            HologramStyle[ItemID.Excalibur] = ([new Color(236, 200, 19)], "Excalibur", false, 0);
            HologramStyle[ItemID.TrueExcalibur] = ([new Color(236, 200, 19)], "TrueExcalibur", true, 0);
            HologramStyle[ItemID.AmethystStaff] = ([new Color(165, 0, 236)], "Amethyst", false, 0);
            HologramStyle[ItemID.AmethystHook] = ([new Color(165, 0, 236)], "Amethyst", false, 0);
            HologramStyle[ItemID.LargeAmethyst] = ([new Color(165, 0, 236)], "Amethyst", false, 0);
            HologramStyle[ItemID.PurplePhasesaber] = ([new Color(165, 0, 236)], "Amethyst", false, 0);
            HologramStyle[ItemID.SapphireStaff] = ([new Color(23, 147, 134)], "Sapphire", false, 0);
            HologramStyle[ItemID.SapphireHook] = ([new Color(23, 147, 134)], "Sapphire", false, 0);
            HologramStyle[ItemID.LargeSapphire] = ([new Color(23, 147, 134)], "Sapphire", false, 0);
            HologramStyle[ItemID.BluePhasesaber] = ([new Color(23, 147, 134)], "Sapphire", false, 0);
            HologramStyle[ItemID.RubyStaff] = ([new Color(238, 51, 53)], "Ruby", false, 0);
            HologramStyle[ItemID.RubyHook] = ([new Color(238, 51, 53)], "Ruby", false, 0);
            HologramStyle[ItemID.LargeRuby] = ([new Color(238, 51, 53)], "Ruby", false, 0);
            HologramStyle[ItemID.RedPhasesaber] = ([new Color(238, 51, 53)], "Ruby", false, 0);
            HologramStyle[ItemID.EmeraldStaff] = ([new Color(33, 184, 115)], "Emerald", false, 0);
            HologramStyle[ItemID.EmeraldHook] = ([new Color(33, 184, 115)], "Emerald", false, 0);
            HologramStyle[ItemID.LargeEmerald] = ([new Color(33, 184, 115)], "Emerald", false, 0);
            HologramStyle[ItemID.GreenPhasesaber] = ([new Color(33, 184, 115)], "Emerald", false, 0);
            HologramStyle[ItemID.TopazStaff] = ([new Color(255, 198, 0)], "Topaz", false, 0);
            HologramStyle[ItemID.TopazHook] = ([new Color(255, 198, 0)], "Topaz", false, 0);
            HologramStyle[ItemID.LargeTopaz] = ([new Color(255, 198, 0)], "Topaz", false, 0);
            HologramStyle[ItemID.YellowPhasesaber] = ([new Color(255, 198, 0)], "Topaz", false, 0);
            HologramStyle[ItemID.DiamondStaff] = ([new Color(155, 200, 202), new Color(218, 185, 210),
                new Color(137, 126, 187)], "Diamond", false, 0);
            HologramStyle[ItemID.DiamondHook] = ([new Color(155, 200, 202), new Color(218, 185, 210),
                new Color(137, 126, 187)], "Diamond", false, 0);
            HologramStyle[ItemID.LargeDiamond] = ([new Color(155, 200, 202), new Color(218, 185, 210),
                new Color(137, 126, 187)], "Diamond", false, 0);
            HologramStyle[ItemID.WhitePhasesaber] = ([new Color(155, 200, 202), new Color(218, 185, 210),
                new Color(137, 126, 187)], "Diamond", false, 0);
            HologramStyle[ItemID.FlowerofFire] = ([new Color(182, 47, 0)], "FlowerOfFire", false, 0);
            HologramStyle[ItemID.FlowerofFrost] = ([new Color(64, 134, 207)], "FlowerOfFrost", false, 0);

            // Soon
            /*if (ModContent.TryFind("lenen", "Tasouken", out ModItem tasouken))
            {
                HologramStyle[tasouken.Type] = ([new Color(255, 0, 0), new Color(0, 255, 0), 
                    new Color(0, 0, 255)], 
                    "TrueExcalibur", true, 0);
            }*/
        }

        public override void PreUpdateTime()
        {
        }

        public override void PostUpdateEverything()
        {
            ColorChangeTimer += 0.003333f;

            if (ColorChangeTimer >= 1)
            {
                ColorChangeTimer = 0;
            }
        }
    }
}
