using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;

namespace MischiefFramework.States {
    class MenuState : IState {
        private SpriteBatch renderTarget;
        private SpriteFont font;

        private Texture2D background;
        private Rectangle bgRect = new Rectangle(0, 0, Game.device.Viewport.Width, Game.device.Viewport.Height);

        private Core.Helpers.MenuHelper menu;

        private delegate bool ActiveDelegate();
        private delegate void StartDelegate();
        private delegate void BackDelegate();

        private Viewport nathanViewport;

        public MenuState(GraphicsDevice device) {
            renderTarget = new SpriteBatch(device);
            background = Cache.ResourceManager.LoadAsset<Texture2D>("HUD/menu");
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");

            nathanViewport.X = device.Viewport.X + 50;
            nathanViewport.Y = device.Viewport.Y + device.Viewport.Height / 3;
            nathanViewport.Width = 500;
            nathanViewport.Height = 500;

            menu = new Core.Helpers.MenuHelper(nathanViewport, Core.Helpers.Positions.CENTERLEFT, new BackDelegate(Quit));
            menu.AddTextMenuItem("Play", ref font, Color.White, Color.Red, new StartDelegate(PlayGame));
            menu.AddTextMenuItem("Ultimate Weapon Monde", ref font, Color.White, Color.Red, new StartDelegate(PlayGame2));
            menu.AddTextMenuItem("Settings", ref font, Color.White, Color.Red, new StartDelegate(Settings));
            menu.AddTextMenuItem("Credits", ref font, Color.White, Color.Red, new StartDelegate(Credits));
            menu.AddTextMenuItem("Quit", ref font, Color.White, Color.Red, new StartDelegate(Quit));
            menu.Update(0f);
        }

        public void PlayGame() {
            StateManager.Push(new SetupState());
        }

        public void PlayGame2() {
            SettingManager._skipPhase1 = true;
            StateManager.Push(new SetupState());
        }

        public void Settings() {
            //StateManager.Push(new SettingsState(Game.device));
        }

        public void Credits() {
            StateManager.Push(new CreditsState(Game.device));
        }

        public void Quit() {
            Game.instance.Exit();
        }

        public bool Update(GameTime gameTime) {
            menu.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();
            renderTarget.Draw(background, bgRect, Color.White);
            renderTarget.End();
            menu.Render(renderTarget);
            return false;
        }

        public bool OnRemove() {
            renderTarget.Dispose();
            return true; //All cleaned up
        }
    }
}
