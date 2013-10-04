using System;
using Microsoft.Xna.Framework;
using MischiefFramework.Cache;
using MischiefFramework.Core;
using MischiefFramework.WorldX.Containers;

namespace MischiefFramework.States {
    internal class PlayingState : IState {
        private Camera mainCam;

        private WorldController worldController;

        public PlayingState() {
            Renderer.Initialize();

            //mainCam = new Camera(64, 36);
            mainCam = new Camera(48, 27);
            //mainCam = new Camera(32, 18);
            //mainCam = new Camera(16, 9);
            //mainCam = new Camera(12, 6.75f);
            //mainCam = new Camera(8, 4.5f);
            //mainCam = new Camera(4, 2.25f);0
            //mainCam = new Camera(2, 1.125f);
            //mainCam = new Camera(1, 0.5625f);

            //Square rendering camera
            //mainCam = new Camera(35, 35);

            Renderer.CharacterCamera = mainCam;

            MischiefFramework.Core.Renderer.Add(new MischiefFramework.WorldX.Information.InfoPanel());

            worldController = new WorldController();
        }

        public bool Update(GameTime gameTime) {
            Renderer.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //TODO: Update things here
            worldController.Update(dt);
            AssetManager.Update(dt);

            return false;
        }

        public bool Render(GameTime gameTime) {
            Renderer.Render();
            return false;
        }

        public bool OnRemove() {
            worldController.Dispose();
            ResourceManager.Flush();
            AssetManager.Flush();
            Renderer.ClearAll();

            return true;
        }
    }
}
