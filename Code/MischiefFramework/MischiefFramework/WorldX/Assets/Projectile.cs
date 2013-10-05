using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Cache;
using MischiefFramework.Core.Interfaces;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;
using FarseerPhysics.Factories;

namespace MischiefFramework.WorldX.Assets {
    class Projectile : Asset, IOpaque {
        private Body body;

        private Model model;

        private Matrix postmultiplied;

        private const float SPEED = 20f;

        private const float LIFESPAN = 10.0f;

        private float timer = 0.0f;

        private float angle = 0.0f;

        private float radius = 0.5f;

        private float heightOffGround = 2.0f;

        public Projectile(World w, float angle, Vector2 position) {
            this.angle = angle;

            model = ResourceManager.LoadAsset<Model>("Meshes/TestObjects/ball");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateCircle(w, radius, 1.0f, position, this);
            body.BodyType = BodyType.Dynamic;
            body.IsBullet = true;
            body.IsSensor = true;

            postmultiplied = Matrix.CreateScale(radius) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        public override void Dispose() {
            body.Dispose();

            Renderer.Remove(this);
        }

        public override void AsyncUpdate(float dt) {
            postmultiplied = Matrix.CreateScale(radius) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * SPEED;

            if (timer <= LIFESPAN) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied, model);
        }
    }
}
