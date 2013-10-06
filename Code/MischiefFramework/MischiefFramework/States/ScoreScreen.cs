using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MischiefFramework.Cache;
using MischiefFramework.Core;

namespace MischiefFramework.States {
    class ScoreScreen : IState {
        private SpriteBatch renderTarget;
        private SpriteFont font;
        private Texture2D bg;
        private Texture2D playerIcons;

        private Vector2 p1Pos, p2Pos, p3Pos, p4Pos;

        public ScoreScreen() {
            renderTarget = new SpriteBatch(Game.device);
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont24");
            bg = ResourceManager.LoadAsset<Texture2D>("HUD/Winners screen");
            playerIcons = ResourceManager.LoadAsset<Texture2D>("HUD/Player Markers");

            p1Pos = new Vector2(600, 200);
            p2Pos = new Vector2(350, 275);
            p3Pos = new Vector2(875, 375);
            p4Pos = new Vector2(0, 0);
        }

        public bool Update(GameTime gameTime) {

            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();
            renderTarget.Draw(bg, renderTarget.GraphicsDevice.Viewport.Bounds, bg.Bounds, Color.White);
            /*for (int i = 0; i < PlayerManager.playerDeathOrder.Count; i++) {

            }*/
            p4Pos.X = 1050;
            p4Pos.Y = 180;
            if (PlayerManager.playerDeathOrder.Count >= 1) {
                renderTarget.Draw(playerIcons, new Rectangle((int)p1Pos.X, (int)p1Pos.Y, 64, 64), new Rectangle(64 * PlayerManager.playerDeathOrder[PlayerManager.playerDeathOrder.Count - 1], 0, 64, 64), Color.White);
            }
            if (PlayerManager.playerDeathOrder.Count >= 2) {
                renderTarget.Draw(playerIcons, new Rectangle((int)p2Pos.X, (int)p2Pos.Y, 64, 64), new Rectangle(64 * PlayerManager.playerDeathOrder[PlayerManager.playerDeathOrder.Count - 2], 0, 64, 64), Color.White);
            }
            if (PlayerManager.playerDeathOrder.Count >= 3) {
                renderTarget.Draw(playerIcons, new Rectangle((int)p3Pos.X, (int)p3Pos.Y, 64, 64), new Rectangle(64 * PlayerManager.playerDeathOrder[PlayerManager.playerDeathOrder.Count - 3], 0, 64, 64), Color.White);
            }
            if (PlayerManager.playerDeathOrder.Count >= 4) {
                renderTarget.Draw(playerIcons, new Rectangle((int)p4Pos.X, (int)p4Pos.Y, 64, 64), new Rectangle(64 * PlayerManager.playerDeathOrder[PlayerManager.playerDeathOrder.Count - 4], 0, 64, 64), Color.White);
            }
            renderTarget.End();
            return false;
        }

        public bool OnRemove() {
            renderTarget.Dispose();
            bg.Dispose();
            return true;
        }
    }
}
