using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using MischiefFramework.WorldX.Stage;
using MischiefFramework.WorldX.Map;
using MischiefFramework.WorldX.Assets;
using MischiefFramework.Cache;

namespace MischiefFramework.WorldX.Containers {
    internal class WorldController : IDisposable {
        internal World world;
        internal Level level;

        public WorldController() {
            world = new FarseerPhysics.Dynamics.World(Vector2.Zero);
            level = new Level(world);

            new Sun();

            //new Character(Player.Input, world);
            new TankCharacter(Player.Input, world);
        }

        public void Update(float dt) {
            //Do nothing?
            world.Step(dt);
        }

        public void Dispose() {
            world.Clear();
            world = null;
        }
    }
}
