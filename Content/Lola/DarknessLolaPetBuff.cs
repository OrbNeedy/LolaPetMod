using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace LolaPet.Content.Lola
{
    public class DarknessLolaPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override string Texture => "LolaPet/Content/Lola/LolaPetBuff";

        public override void Update(Player player, ref int buffIndex)
        {
            int projType = ModContent.ProjectileType<DarknessLolaPetProjectile>();
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, projType);
        }
    }
}
