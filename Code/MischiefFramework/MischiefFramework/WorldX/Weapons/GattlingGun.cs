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

        private Matrix originalWeapon;
        private float spinAngle = 0.0f;

        private const float MGSPREAD = (float)Math.PI/9; // in degrees

        public GattlingGun(TankCharacter myTank) {
            tank = myTank;

            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gattling_Gun");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            originalWeapon = model.Bones[2].Transform;
        }

        public void Update(float dt) {
            if (currentDelay > 0) currentDelay -= dt;
        }

        public void TryFire() {
            if (currentDelay <= 0) {
                // shoot MG
                //1.7X
                //-3.5Z

                Vector3 bulletPos = Vector3.Transform(new Vector3(1.7f, 0, -3.9f), tank.TurretMatrix);
                new MGBullet(tank.body.World, tank.TurretAngle + (float)Game.random.NextDouble()*MGSPREAD, new Vector2(bulletPos.X, bulletPos.Z), tank.player.teamID);

                AudioController.PlayOnce("Gatling_Gun");

                currentDelay = FIRE_DELAY;
            }

            spinAngle += 0.2f;
        }

        public void RenderOpaque() {
            //model.Bones[2].Transform = originalWeapon * Matrix.CreateRotationZ(spinAngle);
            MeshHelper.DrawModel(tank.TurretMatrix, model);
        }
    }
}
