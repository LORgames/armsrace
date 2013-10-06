using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using MischiefFramework.Core;

namespace MischiefFramework.States {
    class SetupState : IState {
        private SpriteBatch renderTarget;
        private SpriteFont font;
        private Texture2D bg;

        private Core.Helpers.MenuHelper menu;

        private delegate bool ActiveDelegate();
        private delegate void StartDelegate();
        private delegate void BackDelegate();

        private delegate int ListDelegate();
        private delegate void ListUpdateDelegate(int value);

        #region Spawn Type Variables
        private List<string> spawnTypes = new List<string>() { "Random", "Center" };
        #endregion

        #region Game Length Variables
        private List<string> gameLengthOptions = new List<string>() { "Quick", "Average", "Physics Lecture" };
        #endregion

        #region Move Speed Variables
        private List<string> moveSpeedOptions = new List<string>() { "Old People", "Average", "Fast" };
        #endregion

        #region Base Sharing Variables
        private List<string> baseSharingOptions = new List<string>() { "No", "Yes" };
        #endregion

        public SetupState() {
            while (PlayerManager.ActivePlayers.Count > 1) PlayerManager.ActivePlayers.RemoveAt(1);
            PlayerManager.ActiveTeams.Clear();
            PlayerManager.deadTeams.Clear();
            PlayerManager.playerDeathOrder.Clear();

            renderTarget = new SpriteBatch(Game.device);
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");
            bg = ResourceManager.LoadAsset<Texture2D>("HUD/options");

            menu = new Core.Helpers.MenuHelper(Game.device.Viewport, Core.Helpers.Positions.CENTER, new BackDelegate(Back));
            menu.SetGapBetweenItems(20.0f);
            menu.AddSublistMenuItem("Spawn Type", ref font, Color.White, Color.Red, Color.Gray, Color.Red, spawnTypes, new ListDelegate(GetSpawnType), new ListUpdateDelegate(UpdateSpawnType));
            menu.AddSublistMenuItem("Game Length", ref font, Color.White, Color.Red, Color.Gray, Color.Red, gameLengthOptions, new ListDelegate(GetGameLength), new ListUpdateDelegate(UpdateGameLength));
            menu.AddSublistMenuItem("Move Speed Modifier", ref font, Color.White, Color.Red, Color.Gray, Color.Red, moveSpeedOptions, new ListDelegate(GetMoveSpeed), new ListUpdateDelegate(UpdateMoveSpeed));
            menu.AddSublistMenuItem("Base Sharing", ref font, Color.White, Color.Red, Color.Gray, Color.Red, baseSharingOptions, new ListDelegate(GetBaseSharing), new ListUpdateDelegate(UpdateBaseSharing));
            menu.AddTextMenuItem("Continue", ref font, Color.White, Color.Red, new StartDelegate(PlayGame));
            menu.AddTextMenuItem("Back", ref font, Color.White, Color.Red, new StartDelegate(Back));
            menu.Update(0f);
        }

        public int GetSpawnType() {
            return SettingManager._spawnType;
        }

        public void UpdateSpawnType(int value) {
            SettingManager._spawnType = value + spawnTypes.Count;
            SettingManager._spawnType %= spawnTypes.Count;
        }

        public int GetGameLength() {
            return SettingManager._gameLength;
        }

        public void UpdateGameLength(int value) {
            SettingManager._gameLength = value + gameLengthOptions.Count;
            SettingManager._gameLength %= gameLengthOptions.Count;
        }

        public int GetMoveSpeed() {
            return SettingManager._moveSpeed;
        }

        public void UpdateMoveSpeed(int value) {
            SettingManager._moveSpeed = value + moveSpeedOptions.Count;
            SettingManager._moveSpeed %= moveSpeedOptions.Count;
        }

        public int GetBaseSharing() {
            return SettingManager._baseSharing;
        }

        public void UpdateBaseSharing(int value) {
            SettingManager._baseSharing = value + baseSharingOptions.Count;
            SettingManager._baseSharing %= baseSharingOptions.Count;
        }

        public void PlayGame() {
            StateManager.Pop();
            StateManager.Push(new LobbyState());
        }

        public void Back() {
            StateManager.Pop();
        }

        public bool Update(GameTime gameTime) {
            menu.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();
            renderTarget.Draw(bg, renderTarget.GraphicsDevice.Viewport.Bounds, bg.Bounds, Color.White);
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
