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

namespace MischiefFramework.WorldX.Containers {
    internal class WorldController : IDisposable {
        internal World world;
        internal Level level;

        public WorldController() {
            world = new FarseerPhysics.Dynamics.World(Vector2.Zero);
            level = new Level(world);

            new Sun();
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
