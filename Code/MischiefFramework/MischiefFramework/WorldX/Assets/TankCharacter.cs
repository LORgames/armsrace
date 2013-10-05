using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using MischiefFramework.Core;
using MischiefFramework.Cache;
using MischiefFramework.Core.Interfaces;
using Microsoft.Xna.Framework;
using MischiefFramework.Core.Helpers;
using FarseerPhysics.Factories;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.WorldX.Assets {
    public class TankCharacter : Character, IOpaque {
        private Model model_tank;
        private Model model_turret;

        private Matrix postmultiplied_tank;
        private Matrix postmultiplied_turret;

        private float turretAngle = 0;

        private const float SPEED = 10f;

        private bool firing = false;

        // guns
        private bool hasMG = false;
        private bool hasLaser = false;
        private bool hasCannon = false;
        private bool hasRocket = false;

        // gun timers (in seconds)
        // weapon reader when timer == 0.0f
        // weapon reload time is interval
        private float mgTimer = 0.0f;
        private float mgInterval = 0.5f;
        private float laserTimer = 0.0f;
        private float laserInterval = 3.0f;
        private float cannonTimer = 0.0f;
        private float cannonInterval = 1.0f;
        private float rocketTimer = 0.0f;
        private float rocketInterval = 6.0f;

        public TankCharacter(GamePlayer player, World w) : base(player, w) {
            body = BodyFactory.CreateRectangle(w, 2.0f, 2.0f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;

            model_tank = ResourceManager.LoadAsset<Model>("Meshes/Character/TankBlob");
            MeshHelper.ChangeEffectUsedByModel(model_tank, Renderer.Effect3D);

            model_turret = ResourceManager.LoadAsset<Model>("Meshes/Character/TankTurret");
            MeshHelper.ChangeEffectUsedByModel(model_turret, Renderer.Effect3D);

            postmultiplied_tank = Matrix.Identity;
            postmultiplied_turret = Matrix.Identity;

            Renderer.Add(this);
        }

        public override void Update(float dt) {
            base.AsyncUpdate(dt);

            float x1 = -Input.AimX();
            float y1 = -Input.AimY();

            if (x1 != 0 && y1 != 0) {
                turretAngle = (float)Math.Atan2(x1, y1);

                // shooting
                if (!firing) {
                    firing = true;
                }
            } else {
                if (firing) {
                    // not shooting
                    firing = false;
                }
            }

            postmultiplied_tank = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 0f, body.Position.Y);
            postmultiplied_turret = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-turretAngle - (float)Math.PI / 4 * 3) * Matrix.CreateTranslation(body.Position.X, 0f, body.Position.Y);

            if (hasMG && mgTimer > 0.0f) {
                mgTimer -= dt;
            }
            if (hasLaser && laserTimer > 0.0f) {
                laserTimer -= dt;
            }
            if (hasCannon && cannonTimer > 0.0f) {
                cannonTimer -= dt;
            }
            if (hasRocket && rocketTimer > 0.0f) {
                rocketTimer -= dt;
            }

            if (firing) {
                if (hasMG && mgTimer <= 0.0f) {
                    mgTimer = mgInterval;

                    // shoot MG
                    Projectile projectile = new Projectile(body.World, body.Rotation);
                    
                }
                if (hasLaser && laserTimer <= 0.0f) {
                    laserTimer = laserInterval;

                    // shoot Laser

                }

                if (hasCannon && cannonTimer <= 0.0f) {
                    cannonTimer = cannonInterval;

                    // shoot Cannon

                }

                if (hasRocket && rocketTimer <= 0.0f) {
                    rocketTimer = rocketInterval;

                    // shoot Rocket

                }
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied_tank, model_tank);
            MeshHelper.DrawModel(postmultiplied_turret, model_turret);
        }
    }
}
