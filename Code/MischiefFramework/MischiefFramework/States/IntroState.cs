using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core;
using MischiefFramework.Cache;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace MischiefFramework.States {
    internal class IntroState : IState {
        private SpriteBatch renderTarget;

        private Texture2D background;
        private VideoPlayer player;
        private Video video;

        public IntroState() {
            renderTarget = new SpriteBatch(Game.device);
            background = Cache.ResourceManager.LoadAsset<Texture2D>("HUD/menu");

            video = ResourceManager.LoadAsset<Video>("HUD/MenuVideo");
            player = new VideoPlayer();
            player.IsLooped = false;
            player.Play(video);
            AudioController.PlayLooped("AudioFile.wma");

            PlayerInput.SetMouseLock(false);
            Game.players.Activate();
        }

        public bool Update(GameTime gameTime) {
            if (PlayerManager.ActivePlayers.Count > 0) {
                Cache.Player.Input = PlayerManager.ActivePlayers[0].Input;
                StateManager.Push(new MenuState(Game.device));
                Game.players.Deactivate();
                AudioController.RemoveAllLoops();
            }

            // Update video
            if (player.IsLooped == false && player.State == MediaState.Stopped) {
                video = ResourceManager.LoadAsset<Video>("HUD/MenuVideoLooped");
                player.IsLooped = true;
                player.Play(video);
            }

            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();
            if (player.State != MediaState.Stopped) {
                background = player.GetTexture();
            }
            renderTarget.Draw(background, renderTarget.GraphicsDevice.Viewport.Bounds, Color.White);

            renderTarget.Draw(background, renderTarget.GraphicsDevice.Viewport.Bounds, Color.White);
            renderTarget.End();

            return false;
        }

        public bool OnRemove() {
            renderTarget.Dispose();
            return true; //All cleaned up
        }
    }
}
