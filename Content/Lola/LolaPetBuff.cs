using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LolaPet.Content.Lola
{
    public class LolaPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int projType = ModContent.ProjectileType<LolaPetProjectile>();
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, projType);
        }
    }
}
