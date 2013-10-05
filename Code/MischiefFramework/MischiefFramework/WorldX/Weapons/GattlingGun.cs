using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Cache;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.WorldX.Assets;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core.Helpers;
using MischiefFramework.Core;
using Microsoft.Xna.Framework;
using MischiefFramework.WorldX.Projectiles;

namespace MischiefFramework.WorldX.Weapons {
    public class GattlingGun : IWeapon {
        private const float FIRE_DELAY = 0.1f;

        private TankCharacter tank;
        private float currentDelay = 0.0f;

        private Model model;

        private const int MGSPREAD = 20; // in degrees

        public GattlingGun(TankCharacter myTank) {
            tank = myTank;

            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gatling Gun");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
        }

        public void Update(float dt) {
            if (currentDelay > 0) currentDelay -= dt;
        }

        public void TryFire() {
            if (currentDelay <= 0) {
                // shoot MG
                Vector3 bulletPos = Vector3.Transform(Vector3.Forward * 5.2f, tank.TurretMatrix);
                new MGBullet(tank.body.World, tank.TurretAngle + (float)Game.random.Next(MGSPREAD), new Vector2(bulletPos.X, bulletPos.Z), tank.player.teamID);

                currentDelay = FIRE_DELAY;
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(tank.TurretMatrix, model);
        }
    }
}
