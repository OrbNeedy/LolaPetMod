using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;

namespace LolaPet.Content.Dusts
{
    public class DarknessDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 30 * Main.rand.Next(4), 18, 18);
            dust.customData = 0;
        }

        public override bool Update(Dust dust)
        {
            if (dust.alpha >= 255)
            {
                dust.active = false;
            }
            dust.position += new Vector2(0, -4);
            dust.alpha++;

            if (dust.customData is int timer)
            {
                dust.frame = new Rectangle(0, 30 * (timer/45), 18, 18);
                timer++;
                if (timer % 3 == 0) dust.alpha--;
                if (timer >= 180)
                {
                    timer = 0;
                }
            }
            return false;
        }
    }
}
