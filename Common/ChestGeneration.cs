using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using LolaPet.Content.Lola;

namespace LolaPet.Common
{
    public class ChestGeneration : ModSystem
    {

        public override void PostWorldGen()
        {
            int gv2Lola = ModContent.ItemType<LolaPetItem>();
            int[] vineChestItems = { };
            int[] spiderChestItems = { };
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                int chestItemsChoice = 0;
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {
                    switch (Main.tile[chest.x, chest.y].TileFrameX)
                    {
                        // Wood chest
                        case 0:
                            break;
                        // Gold chest
                        case 1 * 36:
                            break;
                        // Locked gold chest
                        case 2 * 36:
                            PutInChest(chest, gv2Lola, !Main.rand.NextBool(10));
                            break;
                        // Shadow chest
                        case 3 * 36:
                            break;
                        // Locked Shadow chest
                        case 4 * 36:
                            PutInChest(chest, gv2Lola, !Main.rand.NextBool(10));
                            break;
                        // Vine chest
                        case 12 * 36:
                            //PutInChest(chest, ref chestItemsChoice, vineChestItems, !Main.rand.NextBool(12));
                            break;
                        // Spider chest
                        case 16 * 36:
                            //PutInChest(chest, ref chestItemsChoice, spiderChestItems, !Main.rand.NextBool(12));
                            break;
                        // Ocean chest
                        case 18 * 36:
                            break;
                    }
                }
            }
        }

        private void PutInChest(Chest chest, ref int chestItemsChoice, int[] itemPool, bool skip)
        {
            if (skip) return;
            for (int inventoryIndex = 0; inventoryIndex < chest.item.Length; inventoryIndex++)
            {
                if (chest.item[inventoryIndex].type == ItemID.None)
                {
                    chest.item[inventoryIndex].SetDefaults(itemPool[chestItemsChoice]);
                    chestItemsChoice = (chestItemsChoice + 1) % itemPool.Length;
                    break;
                }
            }
        }

        private void PutInChest(Chest chest, int item, bool skip)
        {
            if (skip) return;
            for (int inventoryIndex = 0; inventoryIndex < chest.item.Length; inventoryIndex++)
            {
                if (chest.item[inventoryIndex].type == ItemID.None)
                {
                    chest.item[inventoryIndex].SetDefaults(item);
                    break;
                }
            }
        }
    }
}
