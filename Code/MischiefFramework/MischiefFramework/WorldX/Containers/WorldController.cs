﻿using System;
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
using MischiefFramework.Core;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.WorldX.Containers {
    internal class WorldController : IDisposable {
        internal World world;
        internal Level level;

        #if DEBUG_PHYSICS
            internal static DebugRenderer dbg;
        #endif

        private int MAX_CRATES = 10;
        private WeaponCrate[] Crates;

        public WorldController() {
            world = new FarseerPhysics.Dynamics.World(Vector2.Zero);

            #if DEBUG_PHYSICS
                dbg = new DebugRenderer(world);
            #endif

            level = new Level(world);

            new Sun();

            foreach (GamePlayer plr in PlayerManager.ActivePlayers) {
                plr.character = new BlobCharacter(plr, world, Level.bases[plr.baseID].CenterPoint);
                //plr.character = new TankCharacter(plr, world, hasCannon:true);
            }

            Vector2 pos = Vector2.Zero;
            Crates = new WeaponCrate[MAX_CRATES];
            for (int i = 0; i < MAX_CRATES; i++) {
                if (SettingManager._spawnType == 0) { //Random
                    pos.X = Game.random.Next(16) - 8;
                    pos.Y = Game.random.Next(16) - 8;
                } else { //Center
                    pos.X = 4 * (float)Math.Cos(Math.PI * 2 / MAX_CRATES * i);
                    pos.Y = 4 * (float)Math.Sin(Math.PI * 2 / MAX_CRATES * i);
                }

                Crates[i] = new WeaponCrate(world, pos);
            }
        }

        public void Update(float dt) {
            //Do nothing?
            world.Step(dt);

            Renderer.CharacterCamera.MakeDoCameraAngleGood();

            float CAMERA_ZOOM = 60.0f;
            //Renderer.CharacterCamera.LookAt = Vector3.Zero;
            Renderer.CharacterCamera.Position.X = CAMERA_ZOOM * 0.612f + Renderer.CharacterCamera.LookAt.X;
            Renderer.CharacterCamera.Position.Y = CAMERA_ZOOM * 0.500f + Renderer.CharacterCamera.LookAt.Y;
            Renderer.CharacterCamera.Position.Z = CAMERA_ZOOM * -0.612f + Renderer.CharacterCamera.LookAt.Z;
            Renderer.CharacterCamera.GenerateMatrices();
        }

        public void Dispose() {
            world.Clear();
            world = null;
        }
    }
}
