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

        private List<Model> model_guns = new List<Model>();

        private Matrix postmultiplied_tank;
        private Matrix postmultiplied_turret;

        private float turretAngle = 0.0f;

        private const float SPEED = 10f;

        // guns
        private bool firing = false;

        private bool hasMG = false;
        private bool hasLaser = false;
        private bool hasCannon = false;
        private bool hasRocket = false;

        private const int MGSPREAD = 20; // in degrees

        // gun timers (in seconds)
        // weapon reader when timer == 0.0f
        // weapon reload time is interval
        private float mgTimer = 0.0f;
        private float mgInterval = 0.1f;
        private float laserTimer = 0.0f;
        private float laserInterval = 3.0f;
        private float cannonTimer = 0.0f;
        private float cannonInterval = 1.0f;
        private float rocketTimer = 0.0f;
        private float rocketInterval = 6.0f;

        // health
        private const float MAXHEALTH = 100.0f;
        private float health = MAXHEALTH;

        public TankCharacter(GamePlayer player, World w, 
            bool hasMG = false, bool hasLaser = false, bool hasCannon = false, bool hasRocket = false)
            : base(player, w) {
            this.hasMG = hasMG;
            this.hasLaser = hasLaser;
            this.hasCannon = hasCannon;
            this.hasRocket = hasRocket;

            body = BodyFactory.CreateRectangle(w, 2.0f, 2.0f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;

            model_tank = ResourceManager.LoadAsset<Model>("Meshes/Character/TankBlob");
            MeshHelper.ChangeEffectUsedByModel(model_tank, Renderer.Effect3D);

            if (hasMG) {
                Model model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gatling Gun");
                MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
                model_guns.Add(model);
            }
            if (hasLaser) {
                Model model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gatling Gun");
                MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
                model_guns.Add(model);
            }
            if (hasCannon) {
                Model model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gatling Gun");
                MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
                model_guns.Add(model);
            }
            if (hasRocket) {
                Model model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gatling Gun");
                MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
                model_guns.Add(model);
            }

            postmultiplied_tank = Matrix.Identity;
            postmultiplied_turret = Matrix.Identity;
            
            Renderer.Add(this);
        }

        public override void Update(float dt) {
            base.Update(dt);

            float x1 = -Input.AimX();
            float y1 = -Input.AimY();

            if (x1 != 0 || y1 != 0) {
                turretAngle = (float)(Math.Atan2(x1, y1) - Math.PI/4);

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
            postmultiplied_turret = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-turretAngle - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 0f, body.Position.Y);

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
                    Vector3 bulletPos = Vector3.Transform(Vector3.Forward * 5.2f, postmultiplied_turret);
                    new MGBullet(body.World, turretAngle + MathHelper.ToRadians((float)Game.random.Next(MGSPREAD)), new Vector2(bulletPos.X, bulletPos.Z));
                }
                if (hasLaser && laserTimer <= 0.0f) {
                    laserTimer = laserInterval;

                    // shoot Laser
                    Vector3 bulletPos = Vector3.Transform(Vector3.Forward * 5.2f, postmultiplied_tank);
                    new LaserBullet(body.World, body.Rotation, new Vector2(bulletPos.X, bulletPos.Z));
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
            foreach (Model model in model_guns) {
                MeshHelper.DrawModel(postmultiplied_turret, model);
            }
        }
    }
}
