using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core;
using MischiefFramework.Cache;

namespace MischiefFramework.States {
    internal class IntroState : IState {
        private SpriteBatch renderTarget;

        private SpriteFont font;

        public IntroState() {
            renderTarget = new SpriteBatch(Game.device);
            font = Cache.ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");

            PlayerInput.SetMouseLock(false);
            Game.players.Activate();
        }

        public bool Update(GameTime gameTime) {
            if (PlayerManager.ActivePlayers.Count > 0) {
                Cache.Player.Input = PlayerManager.ActivePlayers[0].Input;
                StateManager.Push(new MenuState(Game.device));
                Game.players.Deactivate();
            }

            return false;
        }

        public bool Render(GameTime gameTime) {
            Vector2 corner = Vector2.Zero;
            corner.X = Game.device.Viewport.TitleSafeArea.Left;
            corner.Y = Game.device.Viewport.TitleSafeArea.Top;

            renderTarget.Begin();
            renderTarget.DrawString(font, "Press Enter Or Start to begin!", corner, Color.White);

            corner.X = Game.device.Viewport.Width / 2;
            corner.Y = Game.device.Viewport.Height / 2;

            renderTarget.End();

            return false;
        }

        public bool OnRemove() {
            renderTarget.Dispose();
            return true; //All cleaned up
        }
    }
}
