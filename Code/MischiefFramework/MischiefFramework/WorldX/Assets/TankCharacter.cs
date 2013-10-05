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
using MischiefFramework.WorldX.Weapons;

namespace MischiefFramework.WorldX.Assets {
    public class TankCharacter : Character, IOpaque {
        private const float TURN_SPEED = 10f;

        private Model model_tank;

        private List<IWeapon> arms = new List<IWeapon>();

        public Matrix TankMatrix;
        public Matrix TurretMatrix;
        private Matrix TurrentOriginalMatrix;

        public float TurretAngle = 0.0f;

        // health
        private const float MAXHEALTH = 100.0f;
        private float health = MAXHEALTH;

        public TankCharacter(GamePlayer player, World w, 
            bool hasMG = false, bool hasLaser = false, bool hasCannon = false, bool hasRocket = false) : base(player, w) {

                SPEED = 5;

            body = BodyFactory.CreateRectangle(w, 2.0f, 2.0f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;
            body.UserData = this;

            model_tank = ResourceManager.LoadAsset<Model>("Meshes/Character/Blob Tank");
            MeshHelper.ChangeEffectUsedByModel(model_tank, Renderer.Effect3D);

            TurrentOriginalMatrix = model_tank.Bones[13].Transform;

            if (hasMG) arms.Add(new GattlingGun(this));
            if (hasLaser) arms.Add(new LaserGun(this));
            if (hasCannon) arms.Add(new Cannon(this));
            if (hasRocket) arms.Add(new RocketLauncher(this));

            TankMatrix = Matrix.Identity;
            TurretMatrix = Matrix.Identity;
            
            Renderer.Add(this);
        }

        public override void Update(float dt) {
            if (health <= 0.0f) {
                return;
            }

            base.Update(dt);

            float x1 = -Input.AimX();
            float y1 = -Input.AimY();

            bool firing = false;

            if (x1 != 0 || y1 != 0) {
                TurretAngle = (float)(Math.Atan2(x1, y1) - Math.PI/4);

                // shooting
                firing = true;
            }

            TankMatrix = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 0f, body.Position.Y);
            TurretMatrix = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-TurretAngle - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 0f, body.Position.Y);

            foreach (IWeapon w in arms) {
                w.Update(dt);

                if (firing) {
                    w.TryFire();
                }
            }
        }

        public void RenderOpaque() {
            model_tank.Bones[13].Transform = Matrix.CreateRotationY(-TurretAngle) * TurrentOriginalMatrix;

            MeshHelper.DrawModel(TankMatrix, model_tank);

            foreach (IWeapon arm in arms) {
                arm.RenderOpaque();
            }
        }

        public void TakeDamage(float damage) {
            health -= damage;

            if (health <= 0.0f) {
                body.Dispose();
            }
        }
    }
}
