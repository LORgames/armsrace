﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Cache;
using MischiefFramework.WorldX.Assets;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;
using Microsoft.Xna.Framework;
using MischiefFramework.WorldX.Projectiles;

namespace MischiefFramework.WorldX.Weapons {
    public class RocketLauncher : IWeapon {
        private const float FIRE_DELAY = 2.0f;
        private float currentDelay = 0.0f;

        private TankCharacter tank;
        private Model model;

        public RocketLauncher(TankCharacter myTank) {
            tank = myTank;

            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Rocket Pod");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
        }

        public void Update(float dt) {
            if (currentDelay > 0) currentDelay -= dt;
        }

        public void TryFire() {
            // When this fires add following line
            //AudioController.PlayOnce("Rocket_Shot");
            if (currentDelay <= 0) {
                currentDelay = FIRE_DELAY;

                Vector3 bulletPos = Vector3.Transform(new Vector3(-1.64f, 0, -1), tank.TurretMatrix);
                new RocketBullet(tank.body.World, tank.TurretAngle, new Vector2(bulletPos.X, bulletPos.Z), tank.player.teamID);
                AudioController.PlayOnce("Rocket_Shot");
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(tank.TurretMatrix, model);
        }
    }
}
