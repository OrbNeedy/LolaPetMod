using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace LolaPet.Content.Lola
{
    public class LolaPetProjectile :ModProjectile
    {
        private Vector2 offsetToCenter = new Vector2(42, 40);
        private Vector2 targetPosition = Vector2.Zero;
        private float distanceChange = 2f;

        private float speed = 3f;
        private float maneouverability = 0.04f;
        private bool transforming = false;
        private int transformTimer = 0;
        private bool changing = false;
        private int changeTimer = 0;

        private const string bodyRoute = "LolaPet/Content/Lola/LolaPetProjectile";
        private const string hologramRoute = "LolaPet/Content/Lola/HologramStyles/LolaPetHologram";

        /*private const string ultraRoute = "LolaPet/Content/Lola/UltraMode/LolaPetProjectile";
        private const string ix2Route = "LolaPet/Content/Lola/Ix2/LolaPetProjectile";
        private const string darknessRoute = "LolaPet/Content/Lola/DarknessMode/LolaPetProjectile";
        private const string awakenedRoute = "LolaPet/Content/Lola/AwakenedMode/LolaPetProjectile";*/

        int currentIndex = -1;
        int lastIndex = -1;

        public bool ultraMode = false;
        public int wouldBeFrame = 0;
        public int wouldBeMaxFrames = 3;
        public int frameTimer = 0;

        public Vector2 wingPosition;

        static Asset<Texture2D> Default;
        static Asset<Texture2D> Body;
        static Asset<Texture2D> Eyes;
        static Asset<Texture2D> Hologram;
        static Asset<Texture2D> HologramGlowmask;
        static Asset<Texture2D> Paint;

        static Asset<Texture2D> UltraBody;
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
        }

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
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
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.dead && player.active && player.HasBuff<LolaPetBuff>())
            {
                Projectile.timeLeft = 2;
            }

            HandleIndexChange(player);
            Behavior(player);

            if (changeTimer > 0) changeTimer--;
            if (transformTimer > 0) transformTimer--;

            lastIndex = currentIndex;
            Projectile.netUpdate = true;
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
                currentIndex = -1;
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
                changing = true;
                changeTimer = 12;
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

            if (ultraMode) return;
            float angle = Projectile.Center.AngleTo(targetPosition);
            if (dir < 0)
            {
                angle += MathHelper.Pi;
            }

            float distance = Projectile.Center.Distance(targetPosition);
            if (distance > 200f)
            {
                Projectile.rotation = float.Lerp(0, angle + (-MathHelper.PiOver4 * dir), 
                    (distance - 200) * 0.001f);
            }
            else
            {
                Projectile.rotation = float.Lerp(Projectile.rotation, 0, maneouverability);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            offsetToCenter = new Vector2(42, 40);
            SpriteEffects flipping = SpriteEffects.None;
            int dir = (int)(Projectile.Center.X - Main.player[Projectile.owner].Center.X);
            if (dir > 0)
            {
                offsetToCenter = new Vector2(22, 40);
                flipping = SpriteEffects.FlipHorizontally;
            }

            if (!ultraMode)
            {
                if (currentIndex == -1)
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
                else
                {
                    float changePercent = (float)changeTimer / 12f;
                    Color finalColor = Color.Lerp(Color.White, PetStyle.HologramStyle[currentIndex].Item1,
                        1 - changePercent);
                    Main.EntitySpriteDraw(
                        Body.Value,
                        Projectile.Center - Main.screenPosition,
                        Default.Value.Bounds,
                        Lighting.GetColor(Projectile.Center.ToTileCoordinates(), Color.White),
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
                        Lighting.GetColor(Projectile.Center.ToTileCoordinates(), finalColor),
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
                        Default.Value.Bounds,
                        Lighting.GetColor(Projectile.Center.ToTileCoordinates(), Color.White),
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
            }
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
        }

        private void UltraMovement(Player player)
        {
            if (!ultraMode)
            {
                wingPosition = Projectile.position;
                return;
            }
            float velDistanceChange = 2f;

            int dir = player.direction;

            Vector2 desiredCenterRelative = new Vector2(dir * -80, -50f);

            desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 150f * MathHelper.TwoPi) * 1.5f;
            desiredCenterRelative.X += (float)Math.Sin(Main.GameUpdateCount / 300f * MathHelper.TwoPi) * 1.5f;

            Vector2 desiredCenter = Projectile.position + desiredCenterRelative;
            Vector2 betweenDirection = desiredCenter - wingPosition;
            float betweenSQ = betweenDirection.LengthSquared();

            // If too far, set the position
            if (betweenSQ > 1000f * 1000f || betweenSQ < velDistanceChange * velDistanceChange)
            {
                wingPosition = desiredCenter;
            }

            if (betweenDirection != Vector2.Zero)
            {
                wingPosition += betweenDirection * 0.8f;
            }
        }

        private void CheckUltra(Player player)
        {
            frameTimer++;
            if (frameTimer >= 45)
            {
                wouldBeFrame++;
                if (wouldBeFrame >= wouldBeMaxFrames) wouldBeFrame = 0;
                frameTimer = 0;
            }

            if (player.statLife <= player.statLifeMax2)
            {
                // If completely transparent, activate Ultra mode and change the used texture
                if (Projectile.alpha >= 255)
                {
                    Main.NewText("Activating ultra mode", new Color(255, 131, 64));
                    ultraMode = true;
                    Projectile.rotation = 0;
                }
                
                // Decrease transparency while not in Ultra mode, increase it while in Ultra mode
                if (!ultraMode)
                {
                    Projectile.alpha += 10;
                } else
                {
                    Projectile.alpha -= 10;
                }
            } else
            {
                // If completely transparent, deactivate Ultra mode
                if (Projectile.alpha >= 255)
                {
                    Main.NewText("Deactivating ultra mode", new Color(255, 131, 64));
                    ultraMode = false;
                }

                // Decrease transparency while in Ultra mode, increase it while not in Ultra mode
                if (!ultraMode)
                {
                    Projectile.alpha -= 10;
                }
                else
                {
                    Projectile.alpha += 10;
                }
            }

            // Clamp Projectile.alpha so it won't cause trouble when fading in and out of Ultra Mode
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<LolaPetBuff>()))
            {
                Projectile.timeLeft = 60;
            } else
            {
                Projectile.alpha -= 12;
            }
        }

        private void Movement(Player player)
        {
            float velDistanceChange = 2f;

            int dir = player.direction;
            Projectile.direction = Projectile.spriteDirection = dir;

            Vector2 desiredCenterRelative = new Vector2(dir * -45, -60f);

            desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 150f * MathHelper.TwoPi) * 15f;
            desiredCenterRelative.X += (float)Math.Sin(Main.GameUpdateCount / 300f * MathHelper.TwoPi) * 18f;

            Vector2 desiredCenter = player.MountedCenter + desiredCenterRelative;
            Vector2 betweenDirection = desiredCenter - Projectile.Center;
            float betweenSQ = betweenDirection.LengthSquared();


            if (betweenSQ > 1000f * 1000f || betweenSQ < velDistanceChange * velDistanceChange)
            {
                Projectile.Center = desiredCenter;
                Projectile.velocity = Vector2.Zero;
            }

            if (betweenDirection != Vector2.Zero)
            {
                Projectile.velocity = betweenDirection * 0.1f;
            }

            // Do not rotate if it's in Ultra Mode
            if (ultraMode) return;
            if (Projectile.velocity.LengthSquared() > 6f * 6f)
            {
                float rotationVel = Projectile.velocity.X * 0.01f + Projectile.velocity.Y * Projectile.spriteDirection * 0.01f;
                if (Math.Abs(Projectile.rotation - rotationVel) >= MathHelper.Pi)
                {
                    if (rotationVel < Projectile.rotation)
                    {
                        Projectile.rotation -= MathHelper.TwoPi;
                    } else
                    {
                        Projectile.rotation += MathHelper.TwoPi;
                    }
                }

                float rotationInertia = 12f;
                Projectile.rotation = (Projectile.rotation * (rotationInertia - 1f) + rotationVel) / rotationInertia;
            }
            else
            {
                if (Projectile.rotation > MathHelper.Pi)
                {
                    Projectile.rotation -= MathHelper.TwoPi;
                }

                if (Projectile.rotation > -0.005f && Projectile.rotation < 0.005f)
                {
                    Projectile.rotation = 0f;
                } else
                {
                    Projectile.rotation *= 0.96f;
                }
            }
        }
    }
}
