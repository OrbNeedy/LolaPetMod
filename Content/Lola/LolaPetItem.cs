using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

namespace LolaPet.Content.Lola
{
    public class LolaPetItem : ModItem
    {
        int lastIndex = -1;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<LolaPetProjectile>(), ModContent.BuffType<LolaPetBuff>());

            Item.width = 28;
            Item.height = 26;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(0, 0, 50);
        }

        public override void LoadData(TagCompound tag)
        {
        }

        public override void SaveData(TagCompound tag)
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 1800);

            return false;
        }

        public override void UpdateInventory(Player player)
        {

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
        }
    }
}
