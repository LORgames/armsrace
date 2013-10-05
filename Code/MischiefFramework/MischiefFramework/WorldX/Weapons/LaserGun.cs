using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Cache;
using MischiefFramework.WorldX.Assets;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core.Helpers;
using MischiefFramework.Core;
using Microsoft.Xna.Framework;
using MischiefFramework.WorldX.Projectiles;

namespace MischiefFramework.WorldX.Weapons {
    public class LaserGun : IWeapon {
        private const float FIRE_DELAY = 1.0f;
        private float currentDelay = 0.0f;

        private TankCharacter tank;
        private Model model;

        public LaserGun(TankCharacter myTank) {
            tank = myTank;

            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Laser Gun");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
        }

        public void Update(float dt) {
            if (currentDelay > 0) currentDelay -= dt;
        }

        public void TryFire() {
            if (currentDelay <= 0) {
                // shoot Laser
                Vector3 bulletPos = Vector3.Transform(Vector3.Forward * 5.2f, tank.TankMatrix);
                new LaserBullet(tank.body.World, tank.body.Rotation, new Vector2(bulletPos.X, bulletPos.Z), tank.player.teamID);

                AudioController.PlayOnce("Laser_Shot");

                currentDelay = FIRE_DELAY;
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(tank.TankMatrix, model);
        }
    }
}
