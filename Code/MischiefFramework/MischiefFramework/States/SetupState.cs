using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;

namespace MischiefFramework.States {
    class SetupState : IState {
        private SpriteBatch renderTarget;
        private SpriteFont font;

        private Core.Helpers.MenuHelper menu;

        private delegate bool ActiveDelegate();
        private delegate void StartDelegate();
        private delegate void BackDelegate();

        private delegate int ListDelegate();
        private delegate void ListUpdateDelegate(int value);

        #region Spawn Type Variables
        private List<string> spawnTypes = new List<string>() { "Random", "Center" };
        private int spawnType = 1;
        #endregion

        #region Game Length Variables
        private List<string> gameLengthOptions = new List<string>() { "Quick", "Average", "Physics Lecture" };
        private int gameLength = 1;
        #endregion

        #region Move Speed Variables
        private List<string> moveSpeedOptions = new List<string>() { "Old People", "Average", "Fast" };
        private int moveSpeed = 1;
        #endregion

        #region Base Sharing Variables
        private List<string> baseSharingOptions = new List<string>() { "No", "Yes" };
        private int baseSharing = 1;
        #endregion

        public SetupState(GraphicsDevice device) {
            renderTarget = new SpriteBatch(device);
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");

            menu = new Core.Helpers.MenuHelper(device.Viewport, Core.Helpers.Positions.CENTER, new BackDelegate(Back));
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
            return spawnType;
        }

        public void UpdateSpawnType(int value) {
            spawnType = value + spawnTypes.Count;
            spawnType %= spawnTypes.Count;
        }

        public int GetGameLength() {
            return gameLength;
        }

        public void UpdateGameLength(int value) {
            gameLength = value + gameLengthOptions.Count;
            gameLength %= gameLengthOptions.Count;
        }

        public int GetMoveSpeed() {
            return moveSpeed;
        }

        public void UpdateMoveSpeed(int value) {
            moveSpeed = value + moveSpeedOptions.Count;
            moveSpeed %= moveSpeedOptions.Count;
        }

        public int GetBaseSharing() {
            return baseSharing;
        }

        public void UpdateBaseSharing(int value) {
            baseSharing = value + baseSharingOptions.Count;
            baseSharing %= baseSharingOptions.Count;
        }

        public void PlayGame() {
            StateManager.Push(new LobbyState(Game.device));
        }

        public void Back() {
            StateManager.Pop();
        }

        public bool Update(GameTime gameTime) {
            menu.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            return false;
        }

        public bool Render(GameTime gameTime) {
            menu.Render(renderTarget);
            return false;
        }

        public bool OnRemove() {
            renderTarget.Dispose();
            return true; //All cleaned up
        }
    }
}
