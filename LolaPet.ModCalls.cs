using LolaPet.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;

namespace LolaPet
{
    public partial class LolaPet : Mod
    {
        public override object Call(params object[] args)
        {
            try
            {
                if (args is null)
                {
                    throw new ArgumentNullException(nameof(args), "Arguments cannot be null.");
                }

                if (args.Length == 0)
                {
                    throw new ArgumentException("Arguments cannot be empty.");
                }

                string message = args[0] as string;
                if (message == "AddItemHologram")
                {
                    // Expected: Index, Color, Texture, Glowmask, flipped, Frames
                    // Required: Index, Color, Texture, Glowmask
                    if (args.Length < 5)
                    {
                        throw new ArgumentException("Five arguments are required for this call.");
                    }

                    int index = -1;
                    if (args[1] is int itemId)
                    {
                        index = itemId;
                    } else
                    {
                        throw new ArgumentException("The second argument must be an int with the item's index.");
                    }

                    Color[] color = [new Color(253, 26, 114)];
                    if (args[2] is Color[] paintColor)
                    {
                        color = paintColor;
                    } else
                    {
                        Logger.Warn("The color parameter is invalid, and will revert to the default color.");
                    }

                    Texture2D hologram = null;
                    if (args[3] is Texture2D hologramTexture)
                    {
                        hologram = hologramTexture;
                    } else
                    {
                        throw new ArgumentException("The fourth argument must be a Texture2D with the hologram " +
                            "texture.");
                    }

                    Texture2D glowmask = null;
                    if (args[4] is Texture2D glowmaskTexture)
                    {
                        glowmask = glowmaskTexture;
                    }
                    else
                    {
                        throw new ArgumentException("The fifth argument must be a Texture2D with the hologram " +
                            "glowmask texture.");
                    }

                    bool flipped = false;
                    if (args[5] is bool canFlip)
                    {
                        flipped = canFlip;
                    } else
                    {
                        Logger.Info("The flip parameter will be set to default false.");
                    }

                    // Last value is unused for now
                    PetStyle.ForeignHologramStyles[index] = (color, hologram, glowmask, flipped, 0);
                    return "Success adding hologram style.";
                }
            } catch (Exception e)
            {
                Logger.Error("Call error: " + e.StackTrace + e.Message);
                return "Call regected";
            }
            return "Unrecognized call";
        }
    }
}
