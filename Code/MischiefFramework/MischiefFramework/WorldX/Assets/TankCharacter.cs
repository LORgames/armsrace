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

namespace MischiefFramework.WorldX.Assets {
    public class TankCharacter : Asset, IOpaque {
        private Body body;

        private Model model_tank;
        private Model model_turret;

        private Matrix postmultiplied_tank;
        private Matrix postmultiplied_turret;

        private float turretAngle = 0;

        private PlayerInput input;

        private const float SPEED = 10f;

        public TankCharacter(PlayerInput input, World w) {
            model_tank = ResourceManager.LoadAsset<Model>("Meshes/Character/TankBlob");
            MeshHelper.ChangeEffectUsedByModel(model_tank, Renderer.Effect3D);

            model_turret = ResourceManager.LoadAsset<Model>("Meshes/Character/TankTurret");
            MeshHelper.ChangeEffectUsedByModel(model_turret, Renderer.Effect3D);

            postmultiplied_tank = Matrix.Identity;
            postmultiplied_turret = Matrix.Identity;

            body = BodyFactory.CreateCircle(w, 1.0f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;

            Renderer.Add(this);
            AssetManager.AddAsset(this);

            this.input = input;
        }

        public override void AsyncUpdate(float dt) {
            float x = -input.GetX();
            float y = -input.GetY();

            float x1 = -input.AimX();
            float y1 = -input.AimY();

            if (x != 0 || y != 0) {
                body.Rotation = (float)(Math.Atan2(x, y) + Math.PI / 4);
                body.LinearVelocity = Vector2.UnitX * (float)Math.Cos(body.Rotation) * SPEED + Vector2.UnitY * (float)Math.Sin(body.Rotation) * SPEED;
            } else {
                body.AngularVelocity = 0;
                body.LinearVelocity = Vector2.Zero;
            }

            if (x1 != 0 && y1 != 0) {
                turretAngle = (float)Math.Atan2(x1, y1);
            }

            postmultiplied_tank = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 1f, body.Position.Y);
            postmultiplied_turret = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-turretAngle - (float)Math.PI / 4 * 3) * Matrix.CreateTranslation(body.Position.X, 1f, body.Position.Y);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied_tank, model_tank);
            MeshHelper.DrawModel(postmultiplied_turret, model_turret);
        }
    }
}
