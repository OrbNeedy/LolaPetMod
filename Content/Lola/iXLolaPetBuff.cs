using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LolaPet.Content.Lola
{
    public class iXLolaPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override string Texture => "LolaPet/Content/Lola/LolaPetBuff";

        public override void Update(Player player, ref int buffIndex)
        {
            int projType = ModContent.ProjectileType<iXLolaPetProjectile>();
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, projType);
        }
    }
}
