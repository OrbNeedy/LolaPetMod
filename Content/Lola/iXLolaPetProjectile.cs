using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LolaPet.Content.Lola
{
    public class iXLolaPetProjectile : ModProjectile
    {
        private Vector2 offsetToCenter = new Vector2(42, 56);
        private Vector2 targetPosition = Vector2.Zero;

        private Vector2 ultraOffset = new Vector2(24, 36); // 14, 36
        private Vector2 wingOffset = new Vector2(74, 76); // 54, 76

        private float distanceChange = 2f;

        private float speed = 3f;
        private float maneouverability = 0.04f;
        private bool transforming = false;
        private bool turningBack = false;
        private int transformTimer = 0;
        private bool changing = false;
        private int changeTimer = 0;

        private int colorSelected = 0;

        private const string bodyRoute = "LolaPet/Content/Lola/iXLolaPetProjectile";
        private const string hologramRoute = "LolaPet/Content/Lola/HologramStyles/LolaPetHologram";

        private const string ultraRoute = "LolaPet/Content/Lola/UltraStyles/iXUltraLola";
        /*private const string ixRoute = "LolaPet/Content/Lola/UltraStyles/IX";
        private const string ix2Route = "LolaPet/Content/Lola/UltraStyles/IX2";
        private const string darknessRoute = "LolaPet/Content/Lola/UltraStyles/Darkness";
        private const string awakenedRoute = "LolaPet/Content/Lola/UltraStyles/Awakened";
        private const string mayuRoute = "LolaPet/Content/Lola/UltraStyles/Mayu";
        private const string healingRoute = "LolaPet/Content/Lola/UltraStyles/Healing";*/

        int currentIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        int lastIndex = (int)DefaultStyles.Default;

        private bool ultraMode = false;
        // Limit the time the player will have access to this transformation, resets when the player has 1/3 of
        // it's max life
        private int ultraModeTimer = 0;
        // Timer to initiate the blinking
        private int blinkTimer = 0;
        // Timer to control the blinking
        private int blinking = 0;
        // The current frame of the eyes
        private int eyeFrame = 0;
        // True if the current animation for the eyes is of closing the eyes
        private bool closing = false;
        // Timer to control the change of the current frame of the body
        private int bodyFrameTimer = 0;
        // The current frame of the body
        private int bodyFrame = 0;
        // Timer to control the change of the current frame of the wings
        private int wingFrameTimer = 0;
        // The current frame of the wings
        private int wingFrame = 0;
        // The current frame of the hologram
        private int hologramFrame = 0;
        // Timer to control the change of the current frame of the hologram
        private int hologramFrameTimer = 0;

        private Vector2 wingPosition;

        static Asset<Texture2D> Default;
        static Asset<Texture2D> Body;
        static Asset<Texture2D> Eyes;
        static Asset<Texture2D> Hologram;
        static Asset<Texture2D> HologramGlowmask;
        static Asset<Texture2D> Paint;

        static Asset<Texture2D> UltraBody;
        static Asset<Texture2D> UltraEyes;
        static Asset<Texture2D> UltraBodyGlowmask;
        static Asset<Texture2D> UltraWings;
        static Asset<Texture2D> UltraWingsGlowmask;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // Default is static
                Default = ModContent.Request<Texture2D>(bodyRoute + "_Default");
                // Body remains static
                Body = ModContent.Request<Texture2D>(bodyRoute + "_Body");
                // Eyes remain static and aren't affected by lighting
                Eyes = ModContent.Request<Texture2D>(bodyRoute + "_Eyes");
                // Hologram changes sprite, but defaults to _Default
                Hologram = ModContent.Request<Texture2D>(hologramRoute + "_Default");
                // Hologram Glowmask changes with hologram, but glows
                HologramGlowmask = ModContent.Request<Texture2D>(hologramRoute + "_DefaultGlowmask");
                // Paint changes color with Hologram, but doesn't glow
                Paint = ModContent.Request<Texture2D>(bodyRoute + "_Paint");

                UltraBody = ModContent.Request<Texture2D>(ultraRoute);
                UltraEyes = ModContent.Request<Texture2D>(ultraRoute + "_Eyes");
                UltraBodyGlowmask = ModContent.Request<Texture2D>(ultraRoute + "_Glowmask");
                UltraWings = ModContent.Request<Texture2D>(ultraRoute + "_Wings");
                UltraWingsGlowmask = ModContent.Request<Texture2D>(ultraRoute + "_Wings_Glowmask");
            }
        }

        public override void Unload()
        {
            Default = null;
            Body = null;
            Eyes = null;
            Hologram = null;
            HologramGlowmask = null;
            Paint = null;

            currentIndex = (int)DefaultStyles.Default;
            lastIndex = (int)DefaultStyles.Default;
        }

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets
                .SimpleLoop(0, Main.projFrames[Projectile.type], 6)
                .WithOffset(-4, -2f)
                .WithCode(DelegateMethods.CharacterPreview.Float);

            currentIndex = (int)DefaultStyles.Default;
            lastIndex = (int)DefaultStyles.Default;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 34;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;

            currentIndex = (int)DefaultStyles.Default;
            lastIndex = (int)DefaultStyles.Default;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.dead && player.active && player.HasBuff<iXLolaPetBuff>())
            {
                Projectile.timeLeft = 2;
            }

            HandleIndexChange(player);
            Behavior(player);
            HandleTransformation(player);

            if (changeTimer > 0) changeTimer--;
            if (transformTimer > 0) transformTimer--;
            if (ultraModeTimer > 0) ultraModeTimer--;

            lastIndex = currentIndex;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            currentIndex = (int)DefaultStyles.Default;
            lastIndex = (int)DefaultStyles.Default;
        }

        public override bool PreKill(int timeLeft)
        {
            currentIndex = (int)DefaultStyles.Default;
            lastIndex = (int)DefaultStyles.Default;
            return base.PreKill(timeLeft);
        }

        private void HandleIndexChange(Player player)
        {
            if (Main.dedServ) return;

            currentIndex = player.HeldItem.type;
            // If the index is the same, do nothing
            if (currentIndex == lastIndex) return;

            // If there is no key for the item type, set current index to -1
            if (!PetStyle.HologramStyle.ContainsKey(currentIndex))
            {
                currentIndex = (int)DefaultStyles.Default;
                Hologram = ModContent.Request<Texture2D>($"{hologramRoute}_Default");
                HologramGlowmask = ModContent.Request<Texture2D>($"{hologramRoute}_DefaultGlowmask");
            } else
            {
                // If there is a key for the item type, start a change sequence or reset it
                string assetName = PetStyle.HologramStyle[currentIndex].Item2;
                Hologram = ModContent.Request<Texture2D>($"{hologramRoute}_{assetName}");
                HologramGlowmask = ModContent.Request<Texture2D>($"{hologramRoute}_{assetName}Glowmask");
            }

            if (currentIndex != lastIndex)
            {
                if (currentIndex == -1 || lastIndex == -1)
                {
                    changing = true;
                    changeTimer = 12;
                }
                if (PetStyle.HologramStyle[currentIndex].Item2 != PetStyle.HologramStyle[lastIndex].Item2)
                {
                    changing = true;
                    changeTimer = 12;
                }
            }
        }

        private void Behavior(Player player)
        {
            int dir = (int)(Projectile.Center.X - Main.player[Projectile.owner].Center.X);
            if (dir > 0)
            {
                dir = 1;
            } else
            {
                dir = -1;
            }

            if (transforming)
            {
                Projectile.rotation = 0;
                return;
            }

            float xAddition = (float)(-Math.Cos(Main.GameUpdateCount * 0.0045f * MathHelper.Pi) * 10f);
            float yAddition = (float)(-Math.Sin(Main.GameUpdateCount * 0.009f * MathHelper.Pi) * 5f);
            if (player.direction > 0)
            {
                targetPosition = new Vector2(-61 + xAddition, -23f + yAddition);
            } else
            {
                targetPosition = new Vector2(61 + xAddition, -23f + yAddition);
            }

            targetPosition = player.MountedCenter + targetPosition;

            if (targetPosition.Distance(Projectile.Center) <= 0.1f)
            {
                Projectile.Center = targetPosition;
            } else
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, maneouverability);
            }

            if (ultraMode)
            {
                Projectile.rotation = 0;
                return;
            }

            float distance = Math.Abs(Projectile.Center.X - targetPosition.X);
            if (distance > 200f)
            {
                Projectile.rotation = float.Lerp(0, (-MathHelper.PiOver4 * dir), 
                    (distance - 200) * 0.001f);
            }
            else
            {
                Projectile.rotation = float.Lerp(Projectile.rotation, 0, maneouverability);
            }

            // Follow the player while detransforming
            if (turningBack)
            {
                Projectile.rotation = 0;
            }
        }

        private void HandleTransformation(Player player)
        {
            if (!transforming && !ultraMode)
            {
                if (player.statLife <= player.statLifeMax2/3)
                {
                    transforming = true;
                    transformTimer = 60;
                }
            } 
            
            if (transforming)
            {
                bodyFrame = 0 + (120 - transformTimer)/90;
                wingFrame = 0;

                if (transformTimer <= 0)
                {
                    transforming = false;
                    for (int i = 0; i < 75; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.LavaMoss, 
                            new Vector2(Main.rand.NextFloat(0, 9), 0).RotatedByRandom(MathHelper.TwoPi), 1, 
                            Color.Orange, Main.rand.NextFloat(0, 2));
                    }
                    ultraMode = true;
                    ultraModeTimer = 7200;
                }
            }

            if (turningBack)
            {
                if (transformTimer <= 0)
                {
                    turningBack = false;
                }
            }

            if (ultraMode)
            {
                wingFrameTimer--;

                if (wingFrameTimer <= 0)
                {
                    wingFrameTimer = 20;
                    wingFrame++;
                }

                if (wingFrame > 5)
                {
                    wingFrame = 0;
                }

                if (Projectile.Center.Distance(targetPosition) > 120)
                {
                    bodyFrame = 3;
                    if (wingFrame < 4)
                    {
                        wingFrame = 4;
                    }
                } else
                {
                    bodyFrame = 2;
                    if (wingFrame < 1)
                    {
                        wingFrame = 1;
                    }
                    if (wingFrame > 3)
                    {
                        wingFrame = 1;
                    }
                }

                BlinkController();

                if (ultraModeTimer <= 0)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.boss)
                        {
                            ultraModeTimer = 600;
                            return;
                        }
                    }
                    turningBack = true;
                    transformTimer = 60;
                    ultraMode = false;
                }

                if (player.statLife <= player.statLifeMax2 / 3)
                {
                    ultraModeTimer = 7200;
                }
            }
        }

        private void BlinkController()
        {
            blinkTimer++;
            if (blinkTimer >= 300)
            {
                blinkTimer = 0;
                closing = true;
            }

            if (closing)
            {
                if (eyeFrame < 2)
                {
                    blinking++;
                    if (blinking >= 6)
                    {
                        eyeFrame++;
                        blinking = 0;
                    }
                }
                else
                {
                    blinking = 0;
                    closing = false;
                }
            }
            else
            {
                if (eyeFrame > 0)
                {
                    blinking++;
                    if (blinking >= 6)
                    {
                        eyeFrame--;
                        blinking = 0;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            offsetToCenter = new Vector2(42, 40);
            ultraOffset = new Vector2(24, 36);
            wingOffset = new Vector2(74, 76); // 54, 76
            wingPosition = Projectile.position + new Vector2(10, 19);
            SpriteEffects flipping = SpriteEffects.None;
            int dir = (int)(Projectile.Center.X - Main.player[Projectile.owner].Center.X);
            if (dir > 0)
            {
                offsetToCenter = new Vector2(22, 40);
                ultraOffset = new Vector2(14, 36);
                wingOffset = new Vector2(54, 76);
                wingPosition = Projectile.position + new Vector2(26, 19);
                flipping = SpriteEffects.FlipHorizontally;
            }

            //Main.NewText(Main.menuMode);

            if (Main.menuMode != MenuID.Status && Main.menuMode != MenuID.MultiplayerJoining &&
                Main.menuMode != MenuID.Multiplayer)
            {
                Main.EntitySpriteDraw(
                    Default.Value,
                    Projectile.Center - Main.screenPosition,
                    Default.Value.Bounds,
                    Lighting.GetColor(Projectile.Center.ToTileCoordinates(), Color.White),
                    0,
                    new Vector2(42, 40),
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
                return false;
            }


            if (transforming)
            {
                float transparency = ((float)transformTimer / 40f);
                DrawColoredSphere(lightColor * transparency * 0.7f, flipping, transparency * 0.7f);

                DrawUltraMode(lightColor * (1 - transparency), flipping, (1 - transparency));
                return false;
            }

            if (turningBack)
            {
                float transparency = 1 - ((float)transformTimer / 40f);
                DrawColoredSphere(lightColor * transparency * 0.7f, flipping, transparency * 0.7f);

                DrawUltraMode(lightColor * (1 - transparency), flipping, (1 - transparency));
                return false;
            }

            if (!ultraMode)
            {
                if (currentIndex == -1)
                {
                    DrawDefaultSphere(lightColor, flipping);
                }
                else
                {
                    DrawColoredSphere(lightColor, flipping);
                }
            } else
            {
                DrawUltraMode(lightColor, flipping);
            }
            return false;
        }

        private void DrawColoredSphere(Color lightColor, SpriteEffects flipping, float alpha = 1)
        {
            float changePercent = (float)changeTimer / 12f;

            Color targetColor;
            if (PetStyle.HologramStyle[currentIndex].Item1.Length > 1)
            {
                if (PetStyle.ColorChangeTimer == 0)
                {
                    colorSelected++;
                    if (colorSelected >= PetStyle.HologramStyle[currentIndex].Item1.Length)
                    {
                        colorSelected = 0;
                    }
                }

                int nextColor = colorSelected + 1;
                if (nextColor >= PetStyle.HologramStyle[currentIndex].Item1.Length) nextColor = 0;

                Color color1 = PetStyle.HologramStyle[currentIndex].Item1[colorSelected];
                Color color2 = PetStyle.HologramStyle[currentIndex].Item1[nextColor];
                targetColor = Color.Lerp(color1, color2, PetStyle.ColorChangeTimer);
            } else
            {
                targetColor = PetStyle.HologramStyle[currentIndex].Item1[0];
            }

            Color finalColor = Color.Lerp(Color.White, targetColor, 1 - changePercent);

            int verticalFrames = Hologram.Value.Bounds.Height / (int)PetStyle.HologramSize.Y;
            if (verticalFrames > 1)
            {
                hologramFrameTimer++;
                if (hologramFrameTimer >= PetStyle.HologramStyle[currentIndex].Item4)
                {
                    hologramFrame++;
                    if (hologramFrame >= verticalFrames)
                    {
                        hologramFrame = 0;
                    }
                    hologramFrameTimer = 0;
                }
            }
            else
            {
                hologramFrame = 0;
            }

            int flipValue = 0;
            if (flipping == SpriteEffects.None && PetStyle.HologramStyle[currentIndex].Item3)
            {
                flipValue = 1;
            }
            Main.EntitySpriteDraw(
                Body.Value,
                Projectile.Center - Main.screenPosition,
                Default.Value.Bounds,
                lightColor,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                Paint.Value,
                Projectile.Center - Main.screenPosition,
                Default.Value.Bounds,
                // Don't use light color, because the paint is always different
                Lighting.GetColor(Projectile.Center.ToTileCoordinates(), finalColor) * alpha,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                Eyes.Value,
                Projectile.Center - Main.screenPosition,
                Default.Value.Bounds,
                Color.White,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                Hologram.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(
                    (int)PetStyle.HologramSize.X * flipValue,
                    (int)PetStyle.HologramSize.Y * hologramFrame,
                    (int)PetStyle.HologramSize.X,
                    (int)PetStyle.HologramSize.Y),
                lightColor,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                HologramGlowmask.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(
                    (int)PetStyle.HologramSize.X * flipValue,
                    (int)PetStyle.HologramSize.Y * hologramFrame,
                    (int)PetStyle.HologramSize.X,
                    (int)PetStyle.HologramSize.Y),
                Color.White,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );
        }

        private void DrawDefaultSphere(Color lightColor, SpriteEffects flipping)
        {
            float changePercent = (float)changeTimer / 12f;

            Main.EntitySpriteDraw(
                Default.Value,
                Projectile.Center - Main.screenPosition,
                Default.Value.Bounds,
                Lighting.GetColor(Projectile.Center.ToTileCoordinates(), Color.White),
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );

            if (changeTimer > 0)
            {
                Main.EntitySpriteDraw(
                    Paint.Value,
                    Projectile.Center - Main.screenPosition,
                    Default.Value.Bounds,
                    Lighting.GetColor(Projectile.Center.ToTileCoordinates(), Color.White * changePercent),
                    Projectile.rotation,
                    offsetToCenter,
                    Projectile.scale,
                    flipping,
                    0
                );
            }

            Main.EntitySpriteDraw(
                Eyes.Value,
                Projectile.Center - Main.screenPosition,
                Default.Value.Bounds,
                Color.White,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                HologramGlowmask.Value,
                Projectile.Center - Main.screenPosition,
                Default.Value.Bounds,
                Color.White,
                Projectile.rotation,
                offsetToCenter,
                Projectile.scale,
                flipping,
                0
            );
        }

        private void DrawUltraMode(Color lightColor, SpriteEffects flipping, float alpha = 1)
        {
            Main.EntitySpriteDraw(
                UltraWingsGlowmask.Value,
                wingPosition - Main.screenPosition,
                new Rectangle(0, 132 * wingFrame, 128, 132),
                Color.White * alpha,
                0,
                wingOffset,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                UltraWings.Value,
                wingPosition - Main.screenPosition,
                new Rectangle(0, 132 * wingFrame, 128, 132),
                lightColor,
                0,
                wingOffset,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                UltraBody.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(0, 64 * bodyFrame, 38, 64),
                lightColor,
                0,
                ultraOffset,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                UltraEyes.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(38 * eyeFrame, 64 * bodyFrame, 38, 64),
                lightColor,
                0,
                ultraOffset,
                Projectile.scale,
                flipping,
                0
            );

            Main.EntitySpriteDraw(
                UltraBodyGlowmask.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(0, 64 * bodyFrame, 38, 64),
                Color.White * alpha,
                0,
                ultraOffset,
                Projectile.scale,
                flipping,
                0
            );
        }

        public override void PostDraw(Color lightColor)
        {
        }
    }
}
