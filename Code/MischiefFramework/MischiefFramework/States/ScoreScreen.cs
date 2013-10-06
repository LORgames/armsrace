using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MischiefFramework.Cache;

namespace MischiefFramework.States {
    class ScoreScreen : IState {
        private SpriteBatch renderTarget;
        private SpriteFont font;
        private Texture2D bg;

        public ScoreScreen() {
            renderTarget = new SpriteBatch(Game.device);
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont24");
            bg = ResourceManager.LoadAsset<Texture2D>("HUD/Winners screen");
        }

        public bool Update(GameTime gameTime) {

            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();
            renderTarget.Draw(bg, renderTarget.GraphicsDevice.Viewport.Bounds, bg.Bounds, Color.White);
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
