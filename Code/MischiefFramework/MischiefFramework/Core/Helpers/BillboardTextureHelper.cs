using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using Microsoft.Xna.Framework;

namespace MischiefFramework.Core.Helpers {
    class BillboardTextureHelper {
        private static Texture2D playerIconTexture;
        private static Texture2D healthBarTexture;
        private static RenderTarget2D retValPlayerIcon;
        private static RenderTarget2D retValHealthBar;
        private static SpriteBatch sb;

        public static RenderTarget2D GeneratePlayerIconTexture(int playerID, bool hasCrate) {
            if (playerIconTexture == null) {
                playerIconTexture = ResourceManager.LoadAsset<Texture2D>("HUD/Player Markers");
            }

            if (retValPlayerIcon == null) {
                retValPlayerIcon = new RenderTarget2D(Game.device, 128, 64);
            }

            if (sb == null) {
                sb = new SpriteBatch(Game.device);
            }

            Game.device.SetRenderTarget(retValPlayerIcon);

            sb.Begin();
            sb.Draw(playerIconTexture, new Rectangle(0, 0, 64, 64), new Rectangle(64 * (playerID % 4), 0, 64, 64), Color.White);
            if (hasCrate) {
                sb.Draw(playerIconTexture, new Rectangle(64, 0, 64, 64), new Rectangle(64 * 4, 0, 64, 64), Color.White);
            }
            sb.End();

            Game.device.SetRenderTarget(null);

            return retValPlayerIcon;
        }

        public static RenderTarget2D GenerateHealthBar(float current, float max) {
            if (healthBarTexture == null) {
                healthBarTexture = ResourceManager.LoadAsset<Texture2D>("HUD/Info Panel");
            }

            if (retValHealthBar == null) {
                retValHealthBar = new RenderTarget2D(Game.device, 100, 10);
            }

            if (sb == null) {
                sb = new SpriteBatch(Game.device);
            }

            Game.device.SetRenderTarget(retValHealthBar);

            sb.Begin();
            sb.Draw(healthBarTexture, new Rectangle(0, 0, 100, 10), new Rectangle(50, 50, 100, 10), Color.White); // Draw full health bar
            sb.Draw(healthBarTexture, new Rectangle(0, 0, (int)(100f * current / max), 10), new Rectangle(50, 60, (int)(100f * current / max), 10), Color.White); // Draw current health bar
            sb.End();

            Game.device.SetRenderTarget(null);

            return retValHealthBar;
        }
    }
}
