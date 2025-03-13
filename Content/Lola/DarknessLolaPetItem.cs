using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;

namespace LolaPet.Content.Lola
{
    public class DarknessLolaPetItem : ModItem
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
            Item.DefaultToVanitypet(ModContent.ProjectileType<DarknessLolaPetProjectile>(), 
                ModContent.BuffType<DarknessLolaPetBuff>());

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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddTile(TileID.Anvils)
                .AddIngredient<iXLolaPetItem>()
                .AddIngredient(ItemID.Ichor)
                .Register();

            CreateRecipe()
                .AddTile(TileID.Anvils)
                .AddIngredient<iXLolaPetItem>()
                .AddIngredient(ItemID.CursedFlame)
                .Register();

            CreateRecipe()
                .AddTile(TileID.Anvils)
                .AddIngredient<iXLolaPetItem>()
                .AddIngredient(ItemID.TissueSample, 10)
                .Register();

            CreateRecipe()
                .AddTile(TileID.Anvils)
                .AddIngredient<iXLolaPetItem>()
                .AddIngredient(ItemID.ShadowScale, 10)
                .Register();
        }
    }
}
