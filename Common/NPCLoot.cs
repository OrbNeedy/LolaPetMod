using LolaPet.Content.Lola;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace LolaPet.Common
{
    public class NPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.Probe:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LolaPetItem>(), 100));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<iXLolaPetItem>(), 100));
                    break;
                case NPCID.GoblinSorcerer:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarknessLolaPetItem>(), 50));
                    break;
                case NPCID.GoblinSummoner:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarknessLolaPetItem>(), 50));
                    break;
            }
        }
    }
}
