using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.WorldX.Assets;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Cache;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core.Helpers;
using MischiefFramework.Core;
using MischiefFramework.WorldX.Projectiles;
using Microsoft.Xna.Framework;

namespace MischiefFramework.WorldX.Weapons {
    public class Cannon : IWeapon {
        private TankCharacter tank;

        private float currentDelay = 0.0f;
        private const float FIRE_DELAY = 1.0f;

        private Model model;

        public Cannon(TankCharacter myTank) {
            tank = myTank;

            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Cannon");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
        }

        public void Update(float dt) {
            if (currentDelay > 0) currentDelay -= dt;
        }

        public void TryFire() {
            if (currentDelay <= 0) {
                currentDelay = FIRE_DELAY;

                Vector3 bulletPos = Vector3.Transform(Vector3.Forward * 5.2f, tank.TurretMatrix);
                new CannonBullet(tank.body.World, tank.TurretAngle, new Vector2(bulletPos.X, bulletPos.Z), tank.player.teamID);
                AudioController.PlayOnce("Cannon_Fire");
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(tank.TurretMatrix, model);
        }
    }
}
