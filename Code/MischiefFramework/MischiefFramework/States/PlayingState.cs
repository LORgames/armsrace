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

            mainCam = new Camera(48, 27);
            //mainCam = new Camera(32, 18);
            //mainCam = new Camera(16, 9);
            //mainCam = new Camera(12, 6.75f);

            //Square rendering camera
            //mainCam = new Camera(37, 37);

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
