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

        private Texture2D background;
        private Rectangle bgRect = new Rectangle(0, 0, Game.device.Viewport.Width, Game.device.Viewport.Height);

        public IntroState() {
            renderTarget = new SpriteBatch(Game.device);
            background = Cache.ResourceManager.LoadAsset<Texture2D>("HUD/menu");

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
            renderTarget.Begin();
            renderTarget.Draw(background, bgRect, Color.White);
            renderTarget.End();

            return false;
        }

        public bool OnRemove() {
            renderTarget.Dispose();
            return true; //All cleaned up
        }
    }
}
