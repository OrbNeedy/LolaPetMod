using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace LolaPet.Content
{
    public class PetStyle : ModSystem
    {
        public static Dictionary<int, Color> BodyColors;
        public static Dictionary<int, (Color, string)> HologramStyle;

        public override void Load()
        {
            BodyColors = new Dictionary<int, Color>();
            HologramStyle = new Dictionary<int, (Color, string)>();
        }

        public override void PostSetupContent()
        {
            HologramStyle[ItemID.VampireKnives] = (new Color(255, 68, 51), "VampireKnives");
            HologramStyle[ItemID.TerraBlade] = (new Color(34, 177, 76), "Terrablade");
            HologramStyle[ItemID.Minishark] = (new Color(73, 74, 73), "Minishark");
            HologramStyle[ItemID.RazorbladeTyphoon] = (new Color(48, 248, 171), "RazorbladeTyphoon");
            HologramStyle[ItemID.Meowmere] = (new Color(243, 235, 243), "Meowmere");
            HologramStyle[ItemID.DeathSickle] = (new Color(60, 60, 60), "DeathSickle");
            HologramStyle[ItemID.DemonScythe] = (new Color(63, 60, 134), "DemonScythe");
            HologramStyle[ItemID.NightsEdge] = (new Color(139, 42, 156), "NightsEdge");
            HologramStyle[ItemID.EnchantedSword] = (new Color(69, 96, 233), "EnchantedSword");
            HologramStyle[ItemID.Frostbrand] = (new Color(68, 187, 238), "Frostbrand");
        }
    }
}
